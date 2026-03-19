using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

namespace Void.Tests.Integration.Sides.Clients;

public class PortableMinecraftClient : IntegrationSideBase
{
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

        await File.WriteAllTextAsync(dockerfilePath, DockerfileContent, cancellationToken);
        await BuildImageAsync(workingDirectory, cancellationToken);

        return new PortableMinecraftClient();
    }

    public async Task StartConnectingAsync(string address, CancellationToken cancellationToken = default)
    {
        if (_process is { HasExited: false })
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);

        var (host, port) = ParseAddress(address);
        var dockerHost = GetDockerHost(host);
        var networkArgs = GetDockerNetworkArgs();
        var arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
        var volumeName = $"void-tests-portablemc-{arch}";

        var args = new List<string>
        {
            "run", 
            "--rm"
        };
        
        if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
            args.AddRange(["--platform", "linux/amd64"]);
        
        args.AddRange(networkArgs);
        args.Add("-v");
        args.Add($"{volumeName}:/root/.portablemc");
        args.Add(ImageName);
        args.Add("start");
        args.Add("--username");
        args.Add(Username);
        args.Add("--jvm-arg=-Djava.awt.headless=false");
        args.Add("--join-server");
        args.Add(dockerHost);
        args.Add("--join-server-port");
        args.Add(port.ToString());
        args.Add("release");

        Console.WriteLine(string.Join(' ', args));
        StartApplication("docker", hasInput: false, [.. args]);

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
        
        startInfo.ArgumentList.Add("-t");
        startInfo.ArgumentList.Add(ImageName);
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

    private static string GetDockerHost(string host)
    {
        if (OperatingSystem.IsLinux())
            return host;

        return host is "localhost" or "127.0.0.1" ? "host.docker.internal" : host;
    }

    private static string[] GetDockerNetworkArgs()
    {
        if (OperatingSystem.IsLinux())
            return ["--network", "host"];

        return [];
    }

    private static (string host, int port) ParseAddress(string address)
    {
        var lastColon = address.LastIndexOf(':');

        if (lastColon >= 0 && int.TryParse(address[(lastColon + 1)..], out var parsedPort))
            return (address[..lastColon], parsedPort);

        return (address, 25565);
    }

    private const string DockerfileContent = """
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
        """;
}
