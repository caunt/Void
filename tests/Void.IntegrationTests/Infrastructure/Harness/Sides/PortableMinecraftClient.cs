using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Nito.Disposables;
using Void.IntegrationTests.Infrastructure.Extensions;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PortableMinecraftClient(IContainer Container) : IIntegrationSide
{
    private const string Display = ":99";
    private const string RedirectOutput = ">/proc/1/fd/1 2>/proc/1/fd/2";

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } =
    [
        .. ProtocolVersion
        .Range(ProtocolVersion.Oldest, ProtocolVersion.Latest)
        // These do connect to the server before the loading screen completes, causing MC-228828 crash on 1.14-1.19
        .Where(version => version < ProtocolVersion.MINECRAFT_1_14 || version > ProtocolVersion.MINECRAFT_1_19)
    ];

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => Container.ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

    public static async Task<PortableMinecraftClient> CreateAsync(CancellationToken cancellationToken = default)
    {
        var builder = new ContainerBuilder(image: "rust:bookworm")
            .WithEntrypoint("sleep")
            .WithCommand("infinity")
            .WithEnvironment("DEBIAN_FRONTEND", "noninteractive")
            .WithEnvironment("LIBGL_ALWAYS_SOFTWARE", "1")
            .WithEnvironment("DISPLAY", Display);

        if (OperatingSystem.IsLinux())
            builder = builder.WithCreateParameterModifier(parameters => parameters.HostConfig.NetworkMode = "host");

        var container = builder.Build();

        await container.StartAsync(cancellationToken);

        await container.RunCommandAsync($"apt-get update {RedirectOutput}", cancellationToken);
        await container.RunCommandAsync($"apt-get install -y xvfb xfwm4 x11-utils x11-xserver-utils xdotool libasound2 libflite1 libgl1-mesa-dri libxcursor1 libxrandr2 libxi6 libxtst6 libfreetype6 libfontconfig1 ca-certificates imagemagick {RedirectOutput}", cancellationToken);
        await container.RunCommandAsync($"rm -rf /var/lib/apt/lists/* {RedirectOutput}", cancellationToken);

        await container.RunCommandAsync($"cargo install portablemc-cli {RedirectOutput}", cancellationToken);

        // Fix sound errors by using the null audio driver
        await container.RunCommandAsync(@"printf ""pcm.!default { type null }\nctl.!default { type null }\n"" > /etc/asound.conf", cancellationToken);

        // Skip Minecraft accessibility screen
        await container.RunCommandAsync($"mkdir -p \"$HOME/.minecraft\" {RedirectOutput}", cancellationToken);
        await container.RunCommandAsync($"touch \"$HOME/.minecraft/options.txt\" {RedirectOutput}", cancellationToken);

        // Helper script to start display
        await container.RunCommandAsync($$"""
            cat >/usr/local/bin/start-display <<'EOF'
            #!/usr/bin/env bash
            set -e
            export DISPLAY="${DISPLAY:-{{Display}}}"
            rm -f "/tmp/.X${DISPLAY#:}-lock"
            Xvfb "$DISPLAY" -screen 0 1280x720x24 {{RedirectOutput}} &
            until xdpyinfo -display "$DISPLAY" >/dev/null 2>&1; do sleep 0.1; done
            xfwm4 {{RedirectOutput}} &
            EOF
            chmod +x /usr/local/bin/start-display
            """, cancellationToken);

        // Helper script to send chat messages
        await container.RunCommandAsync($$"""
            cat >/usr/local/bin/send-chat <<'EOF'
            #!/usr/bin/env bash
            set -e
            export DISPLAY="${DISPLAY:-{{Display}}}"
            windowId="$(xdotool search --onlyvisible --name "Minecraft" | head -n 1)"
            xdotool windowfocus "$windowId"
            xdotool key --window "$windowId" t
            sleep 1
            xdotool type --window "$windowId" --delay 1 -- "$*"
            xdotool key --window "$windowId" Return
            EOF
            chmod +x /usr/local/bin/send-chat
            """, cancellationToken);

        // Start display
        await container.RunCommandAsync(["start-display"], cancellationToken);

        return new PortableMinecraftClient(container);
    }

    public async Task<Memory<byte>> SendTextMessageAsync(string text, CancellationToken cancellationToken = default)
    {
        return await SendTextMessagesAsync([text], cancellationToken);
    }

    public async Task<Memory<byte>> SendTextMessagesAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        foreach (var text in texts)
        {
            // Is it a Chat Command?
            var expectTask = text.StartsWith('/')
                ? Task.Delay(3_000, cancellationToken) // Give some room delay for client-server reaction
                : Container.ExpectTextAsync(text, cancellationToken); // Expect the text to appear in logs

            await Container.RunCommandAsync(["send-chat", text], cancellationToken);
            await expectTask;
        }

        return await Container.TakeScreenshotAsync(Display, cancellationToken);
    }

    public async Task EnsureStableAsync(CancellationToken cancellationToken = default)
    {
        await EnsureStableAsync(TimeSpan.FromSeconds(10), cancellationToken);
    }

    public async Task EnsureStableAsync(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        // Ensure the game is stable and not doing some background loading
        await Container.WaitForLogsSilenceAsync(duration, whitelist:
        [
            // These are spam messages that can appear because of poor ViaVersion protocol support
            "Unable to play unknown soundEvent", // minecraft:mob.rabbit.hop (or .idle) for example - bunnies often do hop nearby
            " has no item?!", // Item entity 72 has no item?!
            "Skipping Entity with id" // Skipping Entity with id minecraft:cave_spider
        ], cancellationToken);
    }

    public async Task<IAsyncDisposable> RunGameAsync(EndPoint endPoint, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
    {
        await Container.RunCommandAsync($"echo \"Starting Minecraft {protocolVersion.FirstRelease}\" {RedirectOutput}", cancellationToken);

        // Reset whole options.txt (since it is not backwards-compatible, it should reset each time this method executes)
        await Container.RunCommandAsync($"printf \"onboardAccessibility:false\npauseOnLostFocus:false\" > \"$HOME/.minecraft/options.txt\"", cancellationToken);

        var (dockerHost, dockerPort) = endPoint.AsDockerHostPort;

        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var task = Container.RunCommandAsync($"portablemc start --username \"{nameof(PortableMinecraftClient)[..16]}\" --join-server \"{dockerHost}\" --join-server-port \"{dockerPort}\" --jvm-arg=-Djava.awt.headless=false \"{protocolVersion.FirstRelease}\" {RedirectOutput}", cancellationToken, cancellationTokenSource.Token);

        await Container.ExpectTextAsync("Connecting to", cancellationToken);
        await EnsureStableAsync(cancellationToken);

        return AsyncDisposable.Create(async () =>
        {
            await cancellationTokenSource.CancelAsync();

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        });
    }

    public void ClearLogs()
    {
        _readLogsSince = DateTime.UtcNow;
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
