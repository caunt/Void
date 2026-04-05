using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Void.IntegrationTests.Infrastructure.Extensions;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PortableMinecraftClient(IContainer Container) : IIntegrationSide
{
    private const string DockerHost = "host.docker.internal";
    private const string RedirectOutput = ">/proc/1/fd/1 2>/proc/1/fd/2";

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion.Range(ProtocolVersion.Latest, ProtocolVersion.Oldest)];

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => Container.ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

    public static async Task<PortableMinecraftClient> CreateAsync(CancellationToken cancellationToken = default)
    {
        var builder = new ContainerBuilder(image: "rust:bookworm")
            .WithEntrypoint("sleep")
            .WithCommand("infinity")
            .WithEnvironment("DEBIAN_FRONTEND", "noninteractive")
            .WithEnvironment("DISPLAY", ":99")
            .WithEnvironment("LIBGL_ALWAYS_SOFTWARE", "1")
            .WithEnvironment("MESA_GL_VERSION_OVERRIDE", "3.3")
            .WithEnvironment("MESA_GLSL_VERSION_OVERRIDE", "330");

        if (OperatingSystem.IsLinux())
            builder = builder.WithCreateParameterModifier(parameters => parameters.HostConfig.NetworkMode = "host");

        var container = builder.Build();

        await container.StartAsync(cancellationToken);

        await container.RunCommandAsync($"apt-get update {RedirectOutput}", cancellationToken);
        await container.RunCommandAsync($"apt-get install -y xvfb xfwm4 x11-utils xdotool libasound2 libflite1 libgl1-mesa-dri libxcursor1 libxrandr2 libxi6 libxtst6 libfreetype6 libfontconfig1 ca-certificates {RedirectOutput}", cancellationToken);
        await container.RunCommandAsync($"rm -rf /var/lib/apt/lists/* {RedirectOutput}", cancellationToken);

        await container.RunCommandAsync($"cargo install portablemc-cli {RedirectOutput}", cancellationToken);

        // Fix sound errors by using the null audio driver
        await container.RunCommandAsync(@"printf ""pcm.!default { type null }\nctl.!default { type null }\n"" > /etc/asound.conf", cancellationToken);

        // Skip Minecraft accessibility screen
        await container.RunCommandAsync($"mkdir -p \"$HOME/.minecraft\" {RedirectOutput}", cancellationToken);
        await container.RunCommandAsync($"touch \"$HOME/.minecraft/options.txt\" {RedirectOutput}", cancellationToken);

        // Helper script to send chat messages
        await container.RunCommandAsync("""
            cat >/usr/local/bin/send-chat <<'EOF'
            #!/usr/bin/env bash
            set -e
            export DISPLAY="${DISPLAY:-:99}"
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
        await container.RunCommandAsync($"""
            export DISPLAY=:99
            rm -f /tmp/.X99-lock
            Xvfb :99 -screen 0 1280x720x24 {RedirectOutput} &
            until xdpyinfo -display :99 >/dev/null 2>&1; do sleep 0.1; done
            xfwm4 {RedirectOutput} &
            """, cancellationToken);

        return new PortableMinecraftClient(container);
    }

    public Task SendTextMessageAsync(EndPoint endPoint, ProtocolVersion protocolVersion, string text, CancellationToken cancellationToken = default)
    {
        return SendTextMessagesAsync(endPoint, protocolVersion, [text], cancellationToken);
    }

    public async Task SendTextMessagesAsync(EndPoint endPoint, ProtocolVersion protocolVersion, IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        (string host, int port) = endPoint switch
        {
            DnsEndPoint dnsEndPoint when !OperatingSystem.IsLinux() && string.Equals(dnsEndPoint.Host, "localhost", StringComparison.OrdinalIgnoreCase) => (DockerHost, dnsEndPoint.Port),
            DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
            IPEndPoint ipEndPoint when !OperatingSystem.IsLinux() && IPAddress.IsLoopback(ipEndPoint.Address) => (DockerHost, ipEndPoint.Port),
            IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
            _ => throw new NotSupportedException($"Unsupported endpoint type: {endPoint.GetType()}")
        };

        // Reset whole options.txt (since it is not backwards-compatible, it should reset each time this method executes)
        await Container.RunCommandAsync($"printf \"onboardAccessibility:false\n\" > \"$HOME/.minecraft/options.txt\"", cancellationToken);

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var runTask = Container.RunCommandAsync($$"""
            export DISPLAY=:99
            echo "Starting Minecraft {{protocolVersion.VersionIntroducedIn}} & Connecting to {{host}}:{{port}}" {{RedirectOutput}}
            portablemc start --username "{{nameof(PortableMinecraftClient)[..16]}}" --join-server "{{host}}" --join-server-port "{{port}}" --jvm-arg=-Djava.awt.headless=false "{{protocolVersion.VersionIntroducedIn}}" {{RedirectOutput}}
            """, cancellationToken, cancellationTokenSource.Token);

        try
        {
            await ExpectTextAsync("Connecting to", cancellationToken);
            await Container.WaitForLogsSilenceAsync(TimeSpan.FromSeconds(10), cancellationToken);

            foreach (var text in texts)
            {
                await Container.RunCommandAsync(["send-chat", text], cancellationToken);
                await ExpectTextAsync(text, cancellationToken);
            }
        }
        finally
        {
            await cancellationTokenSource.CancelAsync();

            try
            {
                await runTask;
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
        }
    }

    public async Task ExpectTextAsync(string text, CancellationToken cancellationToken = default)
    {
        await ExpectTextAsync(text, lookupHistory: false, cancellationToken);
    }

    public async Task ExpectTextAsync(string text, bool lookupHistory = false, CancellationToken cancellationToken = default)
    {
        var since = lookupHistory ? _readLogsSince : DateTime.UtcNow;

        while (!cancellationToken.IsCancellationRequested)
        {
            var logs = await Container.ReadLogsAsync(since, cancellationToken);

            if (logs.Any(line => line.Contains(text)))
                return;

            await Task.Delay(100, cancellationToken);
        }
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
