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
    private const string ImageName = "void-tests-mc-client";
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

        var args = new List<string> { "run", "--rm" };
        args.AddRange(networkArgs);
        args.Add(ImageName);
        args.Add(dockerHost);
        args.Add(port.ToString());
        args.Add(Username);

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
        FROM node:20-slim

        WORKDIR /app

        RUN npm install minecraft-protocol

        RUN printf '%s\n' \
        'const mc = require("minecraft-protocol");' \
        'const host = process.argv[2] || "localhost";' \
        'const port = parseInt(process.argv[3]) || 25565;' \
        'const username = process.argv[4] || "VoidTestClient";' \
        '' \
        'mc.ping({ host, port }, (err, response) => {' \
        '  if (err) { console.error("Ping failed:", err.message); process.exit(1); }' \
        '  const version = response.version.name;' \
        '  console.log("Server version:", version);' \
        '  const client = mc.createClient({ host, port, username, version, auth: "offline" });' \
        '  client.on("login", () => console.log("Logged in:", username));' \
        '  client.on("error", (err) => { console.error(err.message); process.exit(1); });' \
        '  client.on("end", () => { console.log("Disconnected"); process.exit(0); });' \
        '});' \
        > /app/connect.js

        ENTRYPOINT ["node", "/app/connect.js"]
        """;
}
