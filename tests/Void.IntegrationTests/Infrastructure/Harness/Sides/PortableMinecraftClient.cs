using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Void.IntegrationTests.Infrastructure.Exceptions;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PortableMinecraftClient(IContainer Container) : IIntegrationSide
{
    private const string DockerHost = "host.docker.internal";

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion.Range()];

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

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

        await RunCommandAsync(container, "apt-get update", cancellationToken);
        await RunCommandAsync(container, "apt-get install -y xvfb xfwm4 x11-utils xdotool libasound2 libflite1 libgl1-mesa-dri libxcursor1 libxrandr2 libxi6 libxtst6 libfreetype6 libfontconfig1 ca-certificates", cancellationToken);
        await RunCommandAsync(container, "rm -rf /var/lib/apt/lists/*", cancellationToken);

        await RunCommandAsync(container, "cargo install portablemc-cli", cancellationToken);

        // Fix sound errors by using the null audio driver
        await RunCommandAsync(container, @"printf ""pcm.!default { type null }\nctl.!default { type null }\n"" > /etc/asound.conf", cancellationToken);

        // Skip Minecraft accessibility screen
        await RunCommandAsync(container, "mkdir -p \"$HOME/.minecraft\"", cancellationToken);
        await RunCommandAsync(container, "touch \"$HOME/.minecraft/options.txt\"", cancellationToken);
        await RunCommandAsync(container, """
            if grep -q "^onboardAccessibility:" "$HOME/.minecraft/options.txt"; then
              sed -i "s/^onboardAccessibility:.*/onboardAccessibility:false/" "$HOME/.minecraft/options.txt"
            else
              printf "onboardAccessibility:false\n" >> "$HOME/.minecraft/options.txt"
            fi
            """, cancellationToken);

        // Helper script to send chat messages
        await RunCommandAsync(container, """
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

        var runTask = RunCommandAsync(Container, $$"""
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
                await RunCommandAsync(Container, command: ["send-chat", text], cancellationToken);
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
            var logs = await ReadLogsAsync(since, cancellationToken);

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

    private async Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var (standardOutput, standardError) = await Container.GetLogsAsync(since, ct: cancellationToken);
        return Enumerate(standardError).Prepend("STDERR:").Append("STDOUT:").Concat(Enumerate(standardOutput));
        static IEnumerable<string> Enumerate(string text) => text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(line => line.Trim('\r'));
    }

    private static async Task RunCommandAsync(IContainer container, string[] command, CancellationToken cancellationToken = default)
    {
        var execResult = await container.ExecAsync(command, cancellationToken);

        if (execResult.ExitCode != 0)
            throw new IntegrationTestException($"Exit code {execResult.ExitCode}\nCommand {command}\nSTDOUT:\n{execResult.Stdout}\nSTDERR:\n{execResult.Stderr}");
    }

    private static async Task RunCommandAsync(IContainer container, string command, CancellationToken cancellationToken = default)
    {
        var execResult = await container.ExecAsync(["bash", "-c", command.ReplaceLineEndings("\n")], cancellationToken);

        if (execResult.ExitCode != 0)
            throw new IntegrationTestException($"Exit code {execResult.ExitCode}\nCommand {command}\nSTDOUT:\n{execResult.Stdout}\nSTDERR:\n{execResult.Stderr}");
    }
}
