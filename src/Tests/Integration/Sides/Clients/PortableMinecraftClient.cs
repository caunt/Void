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
    private const string HostCaCertsPath = "/usr/local/share/ca-certificates";
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
        var caCertsDestDir = Path.Combine(workingDirectory, "host-ca-certs");

        if (!Directory.Exists(caCertsDestDir))
            Directory.CreateDirectory(caCertsDestDir);

        if (OperatingSystem.IsLinux() && Directory.Exists(HostCaCertsPath))
        {
            foreach (var certFile in Directory.GetFiles(HostCaCertsPath, "*.crt"))
                File.Copy(certFile, Path.Combine(caCertsDestDir, Path.GetFileName(certFile)), overwrite: true);
        }

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

        if (OperatingSystem.IsLinux() && Directory.Exists(HostCaCertsPath))
        {
            args.Add("-v");
            args.Add($"{HostCaCertsPath}:/host-ca-certs:ro");
        }

        args.Add(ImageName);
        args.Add("--username");
        args.Add(Username);
        args.Add("--jvm-arg=-Djava.awt.headless=false");
        args.Add("--join-server");
        args.Add(dockerHost);
        args.Add("--join-server-port");
        args.Add(port.ToString());
        args.Add("release");
        args.Add("--");
        args.Add("--quickPlayPath");
        args.Add("/tmp/quick-play.json");

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
        FROM debian:bookworm-slim

        ENV DEBIAN_FRONTEND=noninteractive
        ENV DISPLAY=:99
        ENV LIBGL_ALWAYS_SOFTWARE=1
        ENV MESA_GL_VERSION_OVERRIDE=3.3
        ENV MESA_GLSL_VERSION_OVERRIDE=330

        ARG PORTABLEMC_VERSION=5.0.2

        COPY host-ca-certs/ /usr/local/share/ca-certificates/

        RUN apt-get update && apt-get install -y \
            ca-certificates \
         && update-ca-certificates \
         && rm -rf /var/lib/apt/lists/*

        RUN apt-get update && apt-get install -y \
            curl \
            xvfb \
            xfwm4 \
            x11-utils \
            libgl1-mesa-dri \
            libxcursor1 \
            libxrandr2 \
            libxi6 \
            libxtst6 \
            libasound2 \
            libfreetype6 \
            libfontconfig1 \
         && ARCH=$(uname -m) \
         && case "$ARCH" in \
              x86_64)   ARCH_LABEL="x86_64-gnu" ;; \
              aarch64)  ARCH_LABEL="aarch64-gnu" ;; \
              armv7l)   ARCH_LABEL="arm-gnueabihf" ;; \
              i686)     ARCH_LABEL="i686-gnu" ;; \
              *)        echo "Unsupported arch: $ARCH" >&2 && exit 1 ;; \
            esac \
         && curl -fsSL "https://github.com/mindstorm38/portablemc/releases/download/v${PORTABLEMC_VERSION}/portablemc-${PORTABLEMC_VERSION}-linux-${ARCH_LABEL}.tar.gz" \
            | tar xz -C /usr/local/bin --strip-components=1 "portablemc-${PORTABLEMC_VERSION}-linux-${ARCH_LABEL}/portablemc" \
         && rm -rf /var/lib/apt/lists/*

        RUN printf '%s\n' \
        '#!/usr/bin/env bash' \
        'set -e' \
        'if ls /host-ca-certs/*.crt 2>/dev/null 1>&2; then' \
        '    cp /host-ca-certs/*.crt /usr/local/share/ca-certificates/' \
        '    update-ca-certificates 2>/dev/null || true' \
        '    PORTABLEMC_KEYTOOL=$(find "$HOME/.portablemc/jvm" -name "keytool" 2>/dev/null | head -1)' \
        '    if [ -n "$PORTABLEMC_KEYTOOL" ]; then' \
        '        CACERTS=$(find "$HOME/.portablemc/jvm" -name "cacerts" -path "*/security/*" 2>/dev/null | head -1)' \
        '        for cert in /host-ca-certs/*.crt; do' \
        '            alias=$(basename "$cert" .crt | tr -cd "[:alnum:]-" | head -c 40)' \
        '            "$PORTABLEMC_KEYTOOL" -importcert -noprompt -trustcacerts -alias "$alias" -file "$cert" -keystore "$CACERTS" -storepass changeit 2>/dev/null || true' \
        '        done' \
        '    fi' \
        'fi' \
        'Xvfb :99 -screen 0 1280x720x24 &' \
        'sleep 2' \
        'xfwm4 &' \
        'sleep 1' \
        'exec portablemc --main-dir "$HOME/.portablemc" start "$@"' \
        > /entrypoint.sh \
         && chmod +x /entrypoint.sh

        ENTRYPOINT ["/entrypoint.sh"]
        CMD ["release"]
        """;
}

