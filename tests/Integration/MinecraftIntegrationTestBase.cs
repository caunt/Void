namespace Void.Tests.Integration;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Void.Tests.Extensions;
using Xunit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

public abstract class MinecraftIntegrationTestBase : IDisposable
{
    private const string AppName = "Void.Tests";

    private static readonly GitHubClient _gitHubClient = new(new ProductHeaderValue(AppName));
    protected static readonly string WorkingDirectory = Path.Combine(Path.GetTempPath(), AppName, "IntegrationTests");

    protected readonly HttpClient _client = new();

    static MinecraftIntegrationTestBase()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
            _gitHubClient.Credentials = new Credentials(token);
    }

    protected MinecraftIntegrationTestBase()
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");
        _client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

        if (!Directory.Exists(WorkingDirectory))
            Directory.CreateDirectory(WorkingDirectory);
    }

    protected virtual string ExpectedText => "hello void!";

    public async Task RunAsync()
    {
        Process? server = null, client = null;
        var serverLogs = new List<string>();
        var clientLogs = new List<string>();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(180));
        var serverDoneTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        cts.Token.Register(() => serverDoneTcs.TrySetCanceled(cts.Token), useSynchronizationContext: false);

        try
        {

            server = await StartServerAsync(cts.Token);
            var serverTask = HandleOutputAsync(server, HandleServerConsole, cts.Token);

            await serverDoneTcs.Task;

            client = await StartClientAsync(cts.Token);
            var clientTask = HandleOutputAsync(client, HandleClientConsole, cts.Token);

            await serverTask;
            cts.Cancel();

            try
            {
                await clientTask;
            }
            catch (TaskCanceledException)
            {
                // Expected when cancelling after server received expected message
            }
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException(exception.Message + $"\nServer logs:\n{string.Join("\n", serverLogs)}\n\n\nClient logs:\n{string.Join("\n", clientLogs)}", exception);
        }
        finally
        {
            if (client is { HasExited: false })
                await client.ExitAsync();

            if (server is { HasExited: false })
                await server.ExitAsync();
        }

        Assert.Contains(serverLogs, line => line.Contains(ExpectedText));

        bool HandleServerConsole(string line)
        {
            serverLogs.Add(line);

            if (line.Contains("java.lang.UnsupportedClassVersionError"))
                throw new IntegrationTestException("Incompatible Java version for the server");

            if (line.Contains("Done") && line.Contains("For help, type \"help\""))
                serverDoneTcs.SetResult();

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

    protected abstract Task<Process> StartServerAsync(CancellationToken cancellationToken);
    protected abstract Task<Process> StartClientAsync(CancellationToken cancellationToken);

    protected async Task<Process> StartApplicationAsync(string fileName, CancellationToken cancellationToken, params string[] userArguments)
    {
        var arguments = new List<string>(userArguments);
        var protocols = new string[] { "http", "https" };

        var isJar = fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);
        var processStartInfo = new ProcessStartInfo(fileName: isJar ? await SetupJreAsync() : fileName)
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

        foreach (var argument in arguments)
            processStartInfo.ArgumentList.Add(argument);

        return Process.Start(processStartInfo) ?? throw new IntegrationTestException($"Failed to start process for {fileName} with arguments: {string.Join(" ", arguments)}");

        async Task<string> SetupJreAsync()
        {
            var jreWorkingDirectory = Path.Combine(WorkingDirectory, "jre21");
            var javaExecutableName = OperatingSystem.IsWindows() ? "java.exe" : "java";
            var existingJava = Directory.Exists(jreWorkingDirectory)
                ? Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault()
                : null;

            if (existingJava is not null)
                return existingJava;

            if (!Directory.Exists(jreWorkingDirectory))
                Directory.CreateDirectory(jreWorkingDirectory);

            var os = OperatingSystem.IsWindows() ? "windows"
                : OperatingSystem.IsLinux() ? "linux"
                : OperatingSystem.IsMacOS() ? "mac"
                : throw new PlatformNotSupportedException("Unsupported OS");

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

            var javaPath = Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault() ??
                throw new IntegrationTestException("Failed to locate downloaded Java runtime");

            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(javaPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

            var proxyCertificatePath = Environment.GetEnvironmentVariable("CODEX_PROXY_CERT");

            if (!string.IsNullOrWhiteSpace(proxyCertificatePath) && File.Exists(proxyCertificatePath) && Path.GetDirectoryName(javaPath) is { } javaBinariesPath && Directory.GetParent(javaBinariesPath) is { } javaHomePath)
            {
                var keytool = Path.Combine(javaBinariesPath, OperatingSystem.IsWindows() ? "keytool.exe" : "keytool");

                if (File.Exists(keytool))
                {
                    var candidateKeystores = new[]
                    {
                        Path.Combine(javaHomePath.FullName, "lib", "security", "cacerts"),
                        Path.Combine(javaHomePath.FullName, "jre", "lib", "security", "cacerts")
                    };

                    var keystore = candidateKeystores.FirstOrDefault(File.Exists);

                    if (keystore is not null)
                    {
                        var startInfo = new ProcessStartInfo(keytool)
                        {
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        startInfo.ArgumentList.Add("-importcert");
                        startInfo.ArgumentList.Add("-alias");
                        startInfo.ArgumentList.Add("codexproxy");
                        startInfo.ArgumentList.Add("-file");
                        startInfo.ArgumentList.Add(proxyCertificatePath);
                        startInfo.ArgumentList.Add("-keystore");
                        startInfo.ArgumentList.Add(keystore);
                        startInfo.ArgumentList.Add("-storepass");
                        startInfo.ArgumentList.Add("changeit");
                        startInfo.ArgumentList.Add("-noprompt");

                        using var process = Process.Start(startInfo);

                        if (process is not null)
                            await process.WaitForExitAsync(cancellationToken);
                    }
                }
            }

            return javaPath;
        }
    }

    protected async Task DownloadFileAsync(string url, string destination, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(destination);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

    protected static async Task<string> GetGitHubRepositoryLatestReleaseAssetAsync(string ownerName, string repositoryName, Predicate<string> assetFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var latestRelease = await _gitHubClient.Repository.Release.GetLatest(ownerName, repositoryName);
        var asset = latestRelease.Assets.FirstOrDefault(asset => assetFilter(asset.Name));

        Assert.NotNull(asset);

        return asset.BrowserDownloadUrl;
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

    public void Dispose()
    {
        if (Directory.Exists(WorkingDirectory))
            Directory.Delete(WorkingDirectory, true);

        GC.SuppressFinalize(this);
    }
}
