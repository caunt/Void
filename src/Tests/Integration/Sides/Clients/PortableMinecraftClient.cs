using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

namespace Void.Tests.Integration.Sides.Clients;

public class PortableMinecraftClient : IntegrationSideBase
{
    private const string DockerHost = "host.docker.internal";
    private const string ImageName = "minecraft-portablemc-void-tests";
    
    public const string Username = "VoidTestClient";

    private PortableMinecraftClient()
    {
    }

    public static async Task<PortableMinecraftClient> CreateAsync(string workingDirectory, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, nameof(PortableMinecraftClient));

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var dockerfilePath = Path.Combine(workingDirectory, "Dockerfile");

        await File.WriteAllTextAsync(dockerfilePath, 
            """
            FROM rust:bookworm AS builder
            
            RUN cargo install portablemc-cli
            
            FROM debian:bookworm-slim
            
            ENV DEBIAN_FRONTEND=noninteractive
            ENV DISPLAY=:99
            ENV LIBGL_ALWAYS_SOFTWARE=1
            ENV MESA_GL_VERSION_OVERRIDE=3.3
            ENV MESA_GLSL_VERSION_OVERRIDE=330
            
            COPY --from=builder /usr/local/cargo/bin/portablemc /usr/local/bin/portablemc
            
            RUN apt-get update && apt-get install -y \
                xvfb \
                xfwm4 \
                x11-utils \
                xdotool \
                libasound2 \
                libflite1 \
                libgl1-mesa-dri \
                libxcursor1 \
                libxrandr2 \
                libxi6 \
                libxtst6 \
                libasound2 \
                libfreetype6 \
                libfontconfig1 \
                ca-certificates \
             && rm -rf /var/lib/apt/lists/*
            
            RUN printf "pcm.!default { type null }\nctl.!default { type null }\n" > /etc/asound.conf
            
            RUN printf '%s\n' \
            '#!/usr/bin/env bash' \
            'set -e' \
            'export DISPLAY="${DISPLAY:-:99}"' \
            'windowId="$(xdotool search --onlyvisible --name "Minecraft" | head -n 1)"' \
            'xdotool windowactivate --sync "$windowId"' \
            'xdotool key t' \
            'sleep 1' \
            'xdotool type --delay 1 -- "$*"' \
            'xdotool key Return' \
            > /usr/local/bin/send-chat \
             && chmod +x /usr/local/bin/send-chat
            
            RUN printf '%s\n' \
            '#!/usr/bin/env bash' \
            'set -e' \
            'mkdir -p "$HOME/.portablemc"' \
            'touch "$HOME/.portablemc/options.txt"' \
            'if grep -q "^onboardAccessibility:" "$HOME/.portablemc/options.txt"; then' \
            '  sed -i "s/^onboardAccessibility:.*/onboardAccessibility:false/" "$HOME/.portablemc/options.txt"' \
            'else' \
            '  printf "onboardAccessibility:false\n" >> "$HOME/.portablemc/options.txt"' \
            'fi' \
            'Xvfb :99 -screen 0 1280x720x24 &' \
            'sleep 2' \
            'xfwm4 &' \
            'sleep 1' \
            'exec portablemc --main-dir "$HOME/.portablemc" "$@"' \
            > /entrypoint.sh \
             && chmod +x /entrypoint.sh
            
            ENTRYPOINT ["/entrypoint.sh"]
            CMD ["release"]
            """, cancellationToken);
        await BuildImageAsync(workingDirectory, cancellationToken);

        return new PortableMinecraftClient();
    }

    public async Task SendTextMessageAsync(EndPoint endPoint, string text, CancellationToken cancellationToken = default)
    {
        if (_process is { HasExited: false })
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);

        (string host, int port) = endPoint switch
        {
            DnsEndPoint dnsEndPoint when !OperatingSystem.IsLinux() && string.Equals(dnsEndPoint.Host, "localhost", StringComparison.OrdinalIgnoreCase) => (DockerHost, dnsEndPoint.Port),
            DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
            IPEndPoint ipEndPoint when !OperatingSystem.IsLinux() && IPAddress.IsLoopback(ipEndPoint.Address) => (DockerHost, ipEndPoint.Port),
            IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
            _ => throw new NotSupportedException($"Unsupported endpoint type: {endPoint.GetType()}")
        };
        
        var arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
        var volumeName = $"void-tests-portablemc-{arch}";

        // Docker arguments
        var arguments = new List<string> { "run" };
        
        if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
            arguments.AddRange(["--platform", "linux/amd64"]);
        
        if (OperatingSystem.IsLinux())
            arguments.AddRange(["--network", "host"]);
        
        arguments.Add("--rm");
        arguments.AddRange(["-v", $"{volumeName}:/root/.portablemc"]);
        
        arguments.Add(ImageName);
        
        // PortableMC CLI arguments
        arguments.Add("start");
        arguments.AddRange(["--username", Username]);
        arguments.AddRange(["--join-server", host]);
        arguments.AddRange(["--join-server-port", port.ToString()]);
        arguments.Add("--jvm-arg=-Djava.awt.headless=false");
        arguments.Add("release");

        StartApplication("docker", hasInput: false, [.. arguments]);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        if (_process is { HasExited: true })
            throw new IntegrationTestException($"Docker container for {nameof(PortableMinecraftClient)} exited immediately with code {_process.ExitCode}.\nLogs:\n{string.Join("\n", Logs)}");
    }

    private static async Task BuildImageAsync(string workingDirectory, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo("docker")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.ArgumentList.Add("build");
        
        if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
            startInfo.ArgumentList.AddRange(["--platform", "linux/amd64"]);
        
        startInfo.ArgumentList.AddRange(["-t", ImageName]);
        startInfo.ArgumentList.Add(".");

        using var process = Process.Start(startInfo)
            ?? throw new IntegrationTestException("Failed to start Docker image build.");

        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdOut = await stdOutTask;
        var stdErr = await stdErrTask;

        if (process.ExitCode != 0)
            throw new IntegrationTestException($"Docker build failed with exit code {process.ExitCode}.\nSTDOUT:\n{stdOut}\nSTDERR:\n{stdErr}");
    }

    private static async Task RemoveImageAsync(CancellationToken cancellationToken = default)
    {
        using var process = Process.Start(new ProcessStartInfo("docker", ["rmi", "--force", ImageName]));

        if (process is not null)
            await process.WaitForExitAsync(cancellationToken);
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await RemoveImageAsync();
    }
}
