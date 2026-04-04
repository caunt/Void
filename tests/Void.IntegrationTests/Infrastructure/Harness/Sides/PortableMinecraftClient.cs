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

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion.Range()];

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

        await container.RunCommandAsync("apt-get update", cancellationToken);
        await container.RunCommandAsync("apt-get install -y xvfb xfwm4 x11-utils xdotool libasound2 libflite1 libgl1-mesa-dri libxcursor1 libxrandr2 libxi6 libxtst6 libfreetype6 libfontconfig1 ca-certificates", cancellationToken);
        await container.RunCommandAsync("rm -rf /var/lib/apt/lists/*", cancellationToken);

        await container.RunCommandAsync("cargo install portablemc-cli", cancellationToken);

        // Fix sound errors by using the null audio driver
        await container.RunCommandAsync(@"printf ""pcm.!default { type null }\nctl.!default { type null }\n"" > /etc/asound.conf", cancellationToken);

        // Skip Minecraft accessibility screen
        await container.RunCommandAsync("mkdir -p \"$HOME/.minecraft\"", cancellationToken);
        await container.RunCommandAsync("touch \"$HOME/.minecraft/options.txt\"", cancellationToken);
        await container.RunCommandAsync("""
            if grep -q "^onboardAccessibility:" "$HOME/.minecraft/options.txt"; then
              sed -i "s/^onboardAccessibility:.*/onboardAccessibility:false/" "$HOME/.minecraft/options.txt"
            else
              printf "onboardAccessibility:false\n" >> "$HOME/.minecraft/options.txt"
            fi
            """, cancellationToken);

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

        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var runTask = Container.RunCommandAsync($$"""
              export DISPLAY="${DISPLAY:-:99}"
              Xvfb :99 -screen 0 1280x720x24 &
              sleep 2
              xfwm4 &
              sleep 1
              portablemc start --username "{{nameof(PortableMinecraftClient)[..16]}}" --join-server "{{host}}" --join-server-port "{{port}}" --jvm-arg=-Djava.awt.headless=false "{{protocolVersion.VersionIntroducedIn}}" >/proc/1/fd/1 2>/proc/1/fd/2
            """, cancellationToken);

        try
        {
            await ExpectTextAsync("Connecting to", cancellationToken);

            // Wait for silence in the logs
            await Task.Delay(15_000, cancellationToken);

            // while (logConsumer.TimeSinceLastLog <= TimeSpan.FromSeconds(10))
            //     await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            foreach (var text in texts)
            {
                await Container.RunCommandAsync(["send-chat", text], cancellationToken);
                await Task.Delay(3_000, cancellationToken);
            }
        }
        finally
        {
            // TODO: Stop process here
            await runTask;
        }
    }

    public async Task ExpectTextAsync(string text, CancellationToken cancellationToken = default)
    {
        await ExpectTextAsync(text, lookupHistory: false, cancellationToken);
    }

    public async Task ExpectTextAsync(string text, bool lookupHistory = false, CancellationToken cancellationToken = default)
    {
        var since = lookupHistory ? _readLogsSince : DateTime.Now;

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
