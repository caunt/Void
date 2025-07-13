using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Formats.Tar;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using Octokit;
using Void.Tests.Extensions;
using Xunit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace Void.Tests.Integration;

public class MinecraftTests : IDisposable
{
    private const string AppName = "Void.Tests";
    private const string ExpectedText = "hello void!";

    private const string ViaVersionRepositoryOwnerName = "ViaVersion";
    private const string ViaVersionRepositoryName = "ViaVersion";
    private const string ViaBackwardsRepositoryName = "ViaBackwards";

    private const string MinecraftConsoleClientRepositoryOwnerName = "MCCTeam";
    private const string MinecraftConsoleClientRepositoryName = "Minecraft-Console-Client";

    private static readonly GitHubClient _gitHubClient = new(new ProductHeaderValue(AppName));
    private static readonly string _workingDirectory = Path.Combine(Path.GetTempPath(), AppName, "PaperMcIntegrationTests");

    private readonly HttpClient _client = new();

    static MinecraftTests()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
            _gitHubClient.Credentials = new Credentials(token);
    }

    public MinecraftTests()
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");

        // Disable caching to ensure we always get the latest data
        _client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true
        };

        if (!Directory.Exists(_workingDirectory))
            Directory.CreateDirectory(_workingDirectory);
    }

    [Fact]
    public async Task MccConnectsPaperServer()
    {
        Process? server = null, client = null;
        List<string> serverLogs = [], clientLogs = [];

        try
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            var paperJarPath = await SetupPaperServerAsync(cancellationTokenSource.Token);
            var minecraftConsoleClientExecutablePath = await SetupMinecraftConsoleClientAsync(cancellationTokenSource.Token);

            var serverDoneTaskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            cancellationTokenSource.Token.Register(() => serverDoneTaskCompletionSource.TrySetCanceled(cancellationTokenSource.Token), useSynchronizationContext: false);

            try
            {
                server = await StartApplicationAsync(paperJarPath, cancellationTokenSource.Token);
                var serverTask = HandleOutputAsync(server, HandleServerConsole, cancellationTokenSource.Token);

                await serverDoneTaskCompletionSource.Task;

                client = await StartApplicationAsync(minecraftConsoleClientExecutablePath, cancellationTokenSource.Token, "void", "-", "localhost:25565", $"send {ExpectedText}");
                var clientTask = HandleOutputAsync(client, HandleClientConsole, cancellationTokenSource.Token);

                await serverTask;
                cancellationTokenSource.Cancel();

                try
                {
                    await clientTask;
                }
                catch (TaskCanceledException)
                {
                    // Totally expected, we cancel client task when server got expected message
                }
            }
            catch (Exception exception)
            {
                throw new IntegrationTestException(exception.Message + $"\nServer logs:\n{string.Join("\n", serverLogs)}\n\n\nClient logs:\n{string.Join("\n", clientLogs)}", exception);
            }

            Assert.Contains(serverLogs, line => line.Contains(ExpectedText));

            bool HandleServerConsole(string line)
            {
                serverLogs.Add(line);

                if (line.Contains("java.lang.UnsupportedClassVersionError"))
                    throw new IntegrationTestException("Incompatible Java version for the server");

                if (line.Contains("Done") && line.Contains("For help, type \"help\""))
                    serverDoneTaskCompletionSource.SetResult();

                if (line.Contains(ExpectedText))
                    return true;

                return false;
            }

            bool HandleClientConsole(string line)
            {
                clientLogs.Add(line);

                if (line.Contains("Cannot connect to the server : This version is not supported !"))
                    throw new IntegrationTestException("Server / Client version mismatch");

                if (line.Contains("No connection could be made because the target machine actively refused it"))
                    throw new IntegrationTestException("Server is not running or not reachable");

                if (line.Contains("You need to agree to the EULA in order to run the server"))
                    throw new IntegrationTestException("Server EULA not accepted");

                return false;
            }
        }
        finally
        {
            if (client is { HasExited: false })
                await client.ExitAsync();

            if (server is { HasExited: false })
                await server.ExitAsync();
        }
    }

    public void Dispose()
    {
        Directory.Delete(_workingDirectory, true);
        GC.SuppressFinalize(this);
    }

    private static async Task HandleOutputAsync(Process process, Func<string, bool> handler, CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken), useSynchronizationContext: false);

        void Handler(object sender, DataReceivedEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Data != null && handler(eventArgs.Data))
                    taskCompletionSource.SetResult();
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        }

        process.OutputDataReceived += Handler;
        process.ErrorDataReceived += Handler;

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await taskCompletionSource.Task;
        }
        finally
        {
            process.CancelOutputRead();
            process.CancelErrorRead();

            process.OutputDataReceived -= Handler;
            process.ErrorDataReceived -= Handler;
        }
    }

    private async Task<Process> StartApplicationAsync(string fileName, CancellationToken cancellationToken, params string[] userArguments)
    {
        var arguments = new List<string>(userArguments);
        var protocols = new string[] { "http", "https" };

        var isJar = fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);
        var processStartInfo = new ProcessStartInfo(fileName: isJar ? await SetupJreAsync(cancellationToken) : fileName)
        {
            WorkingDirectory = Path.GetDirectoryName(fileName),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            UseShellExecute = false
        };

        foreach (var protocol in protocols)
        {
            var name = protocol + "_proxy";
            var variants = new string[] { name, name.ToUpperInvariant() };

            foreach (var variant in variants)
            {
                var candidate = Environment.GetEnvironmentVariable(variant);

                if (string.IsNullOrWhiteSpace(candidate))
                    continue;

                processStartInfo.Environment[variant] = candidate;

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

        processStartInfo.ArgumentList.AddRange(arguments);

        return Process.Start(processStartInfo) ?? throw new IntegrationTestException($"Failed to start process for {fileName} with arguments: {string.Join(" ", arguments)}");
    }

    private async Task<string> SetupPaperServerAsync(CancellationToken cancellationToken)
    {
        var paperWoringDirectory = Path.Combine(_workingDirectory, "PaperServer");

        if (!Directory.Exists(paperWoringDirectory))
            Directory.CreateDirectory(paperWoringDirectory);

        var versionsJson = await _client.GetStringAsync("https://api.papermc.io/v2/projects/paper", cancellationToken);
        using var versions = JsonDocument.Parse(versionsJson);
        var latestVersion = versions.RootElement.GetProperty("versions").EnumerateArray().Last().GetString();

        var buildsJson = await _client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}", cancellationToken);
        using var builds = JsonDocument.Parse(buildsJson);
        var latestBuild = builds.RootElement.GetProperty("builds").EnumerateArray().Last().GetInt32();

        var buildInfoJson = await _client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}", cancellationToken);
        using var buildInfo = JsonDocument.Parse(buildInfoJson);
        var jarName = buildInfo.RootElement.GetProperty("downloads").GetProperty("application").GetProperty("name").GetString();

        var paperUrl = $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/{jarName}";
        var paperJarPath = Path.Combine(paperWoringDirectory, "paper.jar");

        await DownloadFileAsync(paperUrl, paperJarPath, cancellationToken);
        await SetupCompatibilityPluginsAsync();

        await File.WriteAllTextAsync(Path.Combine(paperWoringDirectory, "eula.txt"), "eula=true", cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(paperWoringDirectory, "server.properties"), "server-port=25565\nonline-mode=false\n", cancellationToken);

        return paperJarPath;

        async Task SetupCompatibilityPluginsAsync()
        {
            var pluginsDirectory = Path.Combine(paperWoringDirectory, "plugins");

            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            var viaVersion = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaVersionRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await DownloadFileAsync(viaVersion, Path.Combine(pluginsDirectory, "ViaVersion.jar"), cancellationToken);

            var viaBackwards = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaBackwardsRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await DownloadFileAsync(viaBackwards, Path.Combine(pluginsDirectory, "ViaBackwards.jar"), cancellationToken);
        }
    }

    private async Task<string> SetupMinecraftConsoleClientAsync(CancellationToken cancellationToken)
    {
        var minecraftConsoleClientWorkingDirectory = Path.Combine(_workingDirectory, "MinecraftConsoleClient");

        if (!Directory.Exists(minecraftConsoleClientWorkingDirectory))
            Directory.CreateDirectory(minecraftConsoleClientWorkingDirectory);

        var path = Path.Combine(minecraftConsoleClientWorkingDirectory, "client");
        var url = await GetGitHubRepositoryLatestReleaseAssetAsync(MinecraftConsoleClientRepositoryOwnerName, MinecraftConsoleClientRepositoryName, name => name.EndsWith(GetMinecraftConsoleClientSuffix()), cancellationToken);

        await DownloadFileAsync(url, path, cancellationToken);

        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        await File.WriteAllTextAsync(Path.Combine(minecraftConsoleClientWorkingDirectory, "MinecraftClient.ini"), $"""
            [Main.Advanced]
            MinecraftVersion = "1.20.4"

            [ChatBot.ScriptScheduler]
            Enabled = true

            [[ChatBot.ScriptScheduler.TaskList]]
            Task_Name = "Task Name 1"
            Trigger_On_Login = true
            Action = "send {ExpectedText}"
            """, cancellationToken);

        return path;

        static string GetMinecraftConsoleClientSuffix()
        {
            var suffixBuilder = new StringBuilder();
            var operatingSystem = OperatingSystem.IsWindows() ? "win" :
                OperatingSystem.IsLinux() ? "linux" :
                OperatingSystem.IsMacOS() ? "osx" :
                throw new PlatformNotSupportedException("Unsupported OS");

            suffixBuilder.Append(operatingSystem);
            suffixBuilder.Append('-');
            suffixBuilder.Append(RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => throw new PlatformNotSupportedException("Unsupported architecture")
            });

            if (operatingSystem == "win")
                suffixBuilder.Append(".exe");

            return suffixBuilder.ToString();
        }
    }

    private async Task DownloadFileAsync(string url, string destination, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(destination);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

    private static async Task<string> GetGitHubRepositoryLatestReleaseAssetAsync(string ownerName, string repositoryName, Predicate<string> assetFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var latestRelease = await _gitHubClient.Repository.Release.GetLatest(ownerName, repositoryName);
        var asset = latestRelease.Assets.FirstOrDefault(asset => assetFilter(asset.Name));

        Assert.NotNull(asset);

        return asset.BrowserDownloadUrl;
    }

    private async Task<string> SetupJreAsync(CancellationToken cancellationToken)
    {
        var jreWorkingDirectory = Path.Combine(_workingDirectory, "jre21");
        var javaExecutableName = OperatingSystem.IsWindows() ? "java.exe" : "java";
        var existingJava = Directory.Exists(jreWorkingDirectory)
            ? Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault()
            : null;

        if (existingJava is not null)
            return existingJava;

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
            "adoptium",
            "temurin21-binaries",
            name => name.Contains($"jre_{arch}_{os}", StringComparison.OrdinalIgnoreCase) && name.EndsWith(extension, StringComparison.OrdinalIgnoreCase),
            cancellationToken);

        var archivePath = Path.Combine(jreWorkingDirectory, Path.GetFileName(url));
        await DownloadFileAsync(url, archivePath, cancellationToken);

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

        var javaPath = Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault();
        if (javaPath is null)
            throw new IntegrationTestException("Failed to locate downloaded Java runtime");

        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(javaPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        return javaPath;
    }
}
