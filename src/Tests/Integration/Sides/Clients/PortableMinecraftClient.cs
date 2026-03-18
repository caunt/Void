using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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

        return new();
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

        var args = new List<string> { "run", "--rm" };
        args.AddRange(networkArgs);
        args.Add("-v");
        args.Add($"{volumeName}:/root/.portablemc");
        args.Add(ImageName);
        args.Add("--auth");
        args.Add($"offline:{Username}");
        args.Add("--");
        args.Add("release");
        args.Add("--server");
        args.Add(dockerHost);
        args.Add("--port");
        args.Add(port.ToString());

        StartApplication("docker", hasInput: false, [.. args]);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        if (_process is { HasExited: true })
            throw new IntegrationTestException($"Docker container for {nameof(PortableMinecraftClient)} exited immediately with code {_process.ExitCode}.\nLogs:\n{string.Join("\n", Logs)}");
    }

    private static async Task BuildImageAsync(string workingDirectory, CancellationToken cancellationToken)
    {
        if (await ImageExistsAsync(cancellationToken))
            return;

        var startInfo = new ProcessStartInfo("docker")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.ArgumentList.Add("build");
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

    private static async Task<bool> ImageExistsAsync(CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo("docker")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.ArgumentList.Add("image");
        startInfo.ArgumentList.Add("inspect");
        startInfo.ArgumentList.Add(ImageName);

        using var process = Process.Start(startInfo);

        if (process is null)
            return false;

        await process.WaitForExitAsync(cancellationToken);
        return process.ExitCode == 0;
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

        COPY --from=builder /usr/local/cargo/bin/portablemc /usr/local/bin/portablemc

        RUN apt-get update && apt-get install -y \
            xvfb \
            xfwm4 \
            x11-utils \
         && rm -rf /var/lib/apt/lists/*

        RUN printf '%s\n' \
        '#!/usr/bin/env bash' \
        'set -e' \
        'Xvfb :99 -screen 0 1280x720x24 &' \
        'sleep 2' \
        'xfwm4 &' \
        'sleep 1' \
        'exec portablemc --main-dir "$HOME/.portablemc" start "$@"' \
        > /entrypoint.sh \
         && chmod +x /entrypoint.sh

        ENTRYPOINT ["/entrypoint.sh"]
        CMD ["--", "release"]
        """;
}
