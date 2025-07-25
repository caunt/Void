using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;
using Xunit;

namespace Void.Tests.Integration.Sides.Clients;

public class HeadlessMcClient : IntegrationSideBase
{
    private const string RepositoryOwnerName = "3arthqu4ke";
    private const string RepositoryName = "HeadlessMC";

    private readonly string _workingDirectory;
    private readonly string _launcherPath;

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [
        .. ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_21_5, ProtocolVersion.MINECRAFT_1_7_2)
    ];

    private HeadlessMcClient(string workingDirectory, string launcherPath, string jrePath)
    {
        _workingDirectory = workingDirectory;
        _launcherPath = launcherPath;
        _jreBinaryPath = jrePath;
    }

    public static async Task<HeadlessMcClient> CreateAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, nameof(HeadlessMcClient));

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var jrePath = await SetupJreAsync(workingDirectory, client, cancellationToken);
        var launcherPath = await DownloadLauncherAsync(workingDirectory, client, cancellationToken);

        return new HeadlessMcClient(workingDirectory, launcherPath, jrePath);
    }

    public async Task SendTextMessageAsync(string address, ProtocolVersion protocolVersion, string text, CancellationToken cancellationToken = default)
    {
        StartProcessWithInput(_launcherPath,
            "launch",
            $"vanilla:{protocolVersion.MostRecentSupportedVersion}",
            "-lwjgl",
            "-offline",
            "-specifics",
            "--game-args",
            $"--quickPlayMultiplayer {address}");

        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start HeadlessMC.");

        try
        {
            await ReceiveOutputAsync(line => line.Contains("joined the game"), cancellationToken);
            await SendCommandAsync($"msg {text}", cancellationToken);
            await Task.Delay(5000, cancellationToken);
            await SendCommandAsync("quit", cancellationToken);
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
        finally
        {
            if (_process is { HasExited: false })
                await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
    }

    private void StartProcessWithInput(string fileName, params string[] userArguments)
    {
        if (_process is { HasExited: false })
            throw new IntegrationTestException($"Process for {fileName} is already running.");

        var arguments = new List<string>(userArguments);
        var protocols = new[] { "http", "https" };

        var isJar = fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);
        var startInfo = new ProcessStartInfo(fileName: isJar ? (File.Exists(_jreBinaryPath) ? _jreBinaryPath! : throw new IntegrationTestException("JRE is not installed")) : fileName)
        {
            WorkingDirectory = Path.GetDirectoryName(fileName),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false
        };

        foreach (var protocol in protocols)
        {
            var name = protocol + "_proxy";
            var variants = new[] { name, name.ToUpperInvariant() };

            foreach (var variant in variants)
            {
                var candidate = Environment.GetEnvironmentVariable(variant);
                if (string.IsNullOrWhiteSpace(candidate))
                    continue;

                startInfo.Environment[variant] = candidate;

                if (!isJar)
                    continue;

                if (!candidate.Contains("://"))
                    candidate = protocol + "://" + candidate;

                if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
                    continue;

                arguments.Add($"-D{protocol}.proxyHost={uri.Host}");
                arguments.Add($"-D{protocol}.proxyPort={uri.Port}");

                break;
            }
        }

        if (isJar)
        {
            arguments.Add("-jar");
            arguments.Add(fileName);
            arguments.Add("--nogui");
        }

        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        _process = Process.Start(startInfo) ?? throw new IntegrationTestException($"Failed to start process for {fileName}");
        void Handler(object? _, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
                _logs.Add(e.Data);
        }
        _process.OutputDataReceived += Handler;
        _process.ErrorDataReceived += Handler;
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    private async Task SendCommandAsync(string command, CancellationToken cancellationToken)
    {
        if (_process?.StandardInput is null)
            throw new IntegrationTestException("Process standard input is not available");

        await _process.StandardInput.WriteLineAsync(command.AsMemory(), cancellationToken);
        await _process.StandardInput.FlushAsync();
    }

    private static async Task<string> DownloadLauncherAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken)
    {
        var url = await GetGitHubRepositoryLatestReleaseAssetAsync(RepositoryOwnerName, RepositoryName, name => name.StartsWith("headlessmc-launcher") && name.EndsWith(".jar"), cancellationToken);
        var path = Path.Combine(workingDirectory, "headlessmc-launcher.jar");
        await client.DownloadFileAsync(url, path, cancellationToken);
        return path;
    }

    private static async Task<string> SetupJreAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken)
    {
        var jreWorkingDirectory = Path.Combine(workingDirectory, "jre21");
        var javaExecutableName = OperatingSystem.IsWindows() ? "java.exe" : "java";
        var existingJava = Directory.Exists(jreWorkingDirectory)
            ? Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault()
            : null;

        if (existingJava is null)
        {
            if (!Directory.Exists(jreWorkingDirectory))
                Directory.CreateDirectory(jreWorkingDirectory);

            var os = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsLinux() ? "linux" : OperatingSystem.IsMacOS() ? "mac" : throw new PlatformNotSupportedException("Unsupported OS");
            var arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm64 => "aarch64",
                Architecture.Arm => "arm",
                _ => throw new PlatformNotSupportedException("Unsupported architecture")
            };

            var extension = OperatingSystem.IsWindows() ? ".zip" : ".tar.gz";
            var url = await GetGitHubRepositoryLatestReleaseAssetAsync(
                ownerName: "adoptium",
                repositoryName: "temurin21-binaries",
                assetFilter: name => name.Contains($"jre_{arch}_{os}", StringComparison.OrdinalIgnoreCase) && name.EndsWith(extension, StringComparison.OrdinalIgnoreCase),
                cancellationToken);

            var archivePath = Path.Combine(jreWorkingDirectory, Path.GetFileName(url));
            await client.DownloadFileAsync(url, archivePath, cancellationToken);

            if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                ZipFile.ExtractToDirectory(archivePath, jreWorkingDirectory);
            }
            else
            {
                await using var fileStream = File.OpenRead(archivePath);
                using var gzip = new GZipStream(fileStream, CompressionMode.Decompress);
                TarFile.ExtractToDirectory(gzip, jreWorkingDirectory, overwriteFiles: true);
            }

            var javaPath = Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault() ?? throw new IntegrationTestException("Failed to locate downloaded Java runtime");

            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(javaPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

            existingJava = javaPath;
        }

        return existingJava;
    }
}
