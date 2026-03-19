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

    private readonly string _workingDirectory;
    private readonly string _dockerContainerName;
    
    private PortableMinecraftClient(string workingDirectory, string dockerContainerName)
    {
        _workingDirectory = workingDirectory;
        _dockerContainerName = dockerContainerName;
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
            'xdotool windowfocus "$windowId"' \
            'xdotool key --window "$windowId" t' \
            'sleep 1' \
            'xdotool type --window "$windowId" --delay 1 -- "$*"' \
            'xdotool key --window "$windowId" Return' \
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

        return new PortableMinecraftClient(workingDirectory, $"void-tests-portablemc-{Random.Shared.Next()}");
    }

    public async Task SendTextMessageAsync(EndPoint endPoint, string text, CancellationToken cancellationToken = default)
    {
        await StopContainerAsync(cancellationToken);

        try
        {
            var potableMcWorkingDirectory = Directory.CreateDirectory(Path.Combine(_workingDirectory, ".portablemc"));
            
            (string host, int port) = endPoint switch
            {
                DnsEndPoint dnsEndPoint when !OperatingSystem.IsLinux() && string.Equals(dnsEndPoint.Host, "localhost", StringComparison.OrdinalIgnoreCase) => (DockerHost, dnsEndPoint.Port),
                DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
                IPEndPoint ipEndPoint when !OperatingSystem.IsLinux() && IPAddress.IsLoopback(ipEndPoint.Address) => (DockerHost, ipEndPoint.Port),
                IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
                _ => throw new NotSupportedException($"Unsupported endpoint type: {endPoint.GetType()}")
            };

            // Docker arguments
            var arguments = new List<string> { "run" };

            if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
                arguments.AddRange(["--platform", "linux/amd64"]);

            if (OperatingSystem.IsLinux())
                arguments.AddRange(["--network", "host"]);

            arguments.AddRange(["--name", _dockerContainerName]);
            arguments.AddRange(["-v", $"{potableMcWorkingDirectory.FullName}:/root/.portablemc"]);
            arguments.Add("--rm");
            arguments.Add("-d");
            arguments.Add(ImageName);

            // PortableMC CLI arguments
            arguments.Add("start");
            arguments.AddRange(["--username", Username]);
            arguments.AddRange(["--join-server", host]);
            arguments.AddRange(["--join-server-port", port.ToString()]);
            arguments.Add("--jvm-arg=-Djava.awt.headless=false");
            arguments.Add("release");

            await RunDockerAsync(arguments, cancellationToken);
            StartApplication("docker", hasInput: false, "logs", "-f", _dockerContainerName);
            
            await ExpectTextAsync("Connecting to", lookupHistory: true, cancellationToken);
            
            if (_process is { HasExited: true })
                throw new IntegrationTestException($"Docker client for {nameof(PortableMinecraftClient)} exited with code {_process.ExitCode}.\nLogs:\n{string.Join("\n", Logs)}");
        }
        finally
        {
            await StopContainerAsync(cancellationToken);
        }
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await StopContainerAsync();
        await RemoveImageAsync();
    }

    private static async Task BuildImageAsync(string workingDirectory, CancellationToken cancellationToken)
    {
        var arguments = new List<string> { "build" };
        
        if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
            arguments.AddRange(["--platform", "linux/amd64"]);
        
        arguments.AddRange(["-t", ImageName]);
        arguments.Add(".");

        await RunDockerAsync(workingDirectory, arguments, cancellationToken);
    }

    private async Task StopContainerAsync(CancellationToken cancellationToken = default)
    {
        if (_process is not { HasExited: false })
            return;
        
        await RunDockerAsync(["stop", _dockerContainerName], cancellationToken);
        await _process.ExitAsync(entireProcessTree: true, cancellationToken);
    }

    private static async Task RemoveImageAsync(CancellationToken cancellationToken = default)
    {
        await RunDockerAsync(["rmi", "--force", ImageName], cancellationToken);
    }
    
    private static async Task RunDockerAsync(IEnumerable<string> arguments, CancellationToken cancellationToken = default)
    {
        await RunDockerAsync(Directory.GetCurrentDirectory(), arguments, cancellationToken);
    }

    private static async Task RunDockerAsync(string workingDirectory, IEnumerable<string> arguments, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo("docker")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        
        processStartInfo.ArgumentList.AddRange(arguments);
        
        using var process = Process.Start(processStartInfo) 
                            ?? throw new IntegrationTestException($"Failed to start: docker {string.Join(' ', processStartInfo.ArgumentList)}.");
        
        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdOut = await stdOutTask;
        var stdErr = await stdErrTask;

        if (process.ExitCode != 0)
            throw new IntegrationTestException($"Docker {string.Join(' ', processStartInfo.ArgumentList)} => failed with exit code {process.ExitCode}.\nSTDOUT:\n{stdOut}\nSTDERR:\n{stdErr}");
    }
}
