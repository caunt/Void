using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Octokit;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

namespace Void.Tests.Integration.Sides;

public abstract class IntegrationSideBase : IIntegrationSide
{
    private const int MaxGitHubReleasesToConsider = 3;
    private const int MaxGitHubReleaseRetries = 5;
    private const int GitHubReleaseRetryDelaySeconds = 20;

    private static readonly AsyncLock _lock = new();
    private static readonly GitHubClient _gitHubClient = new(new ProductHeaderValue($"{nameof(Void)}.{nameof(Tests)}"));

    protected string? _jreBinaryPath;

    protected Process? _process;
    protected readonly List<string> _logs = [];

    public IEnumerable<string> Logs => [.. _logs];

    public void ClearLogs()
    {
        _logs.Clear();
    }

    static IntegrationSideBase()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
            _gitHubClient.Credentials = new Credentials(token);
    }

    public void StartApplication(string fileName, bool hasInput = false, params string[] userArguments)
    {
        if (_process is { HasExited: false })
            throw new IntegrationTestException($"Process for {fileName} is already running.");

        _logs.Clear();

        var arguments = new List<string>(userArguments.Length);

        var isJar = fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);
        var jvmArguments = Enumerable.Empty<string>();
        var jarArguments = userArguments.AsEnumerable();

        if (isJar)
        {
            jvmArguments = userArguments.Where(argument => argument.StartsWith("-D", StringComparison.Ordinal));
            jarArguments = userArguments.Except(jvmArguments);
        }

        var processStartInfo = new ProcessStartInfo(fileName: isJar ? (File.Exists(_jreBinaryPath) ? _jreBinaryPath : throw new IntegrationTestException("JRE is not installed")) : fileName)
        {
            WorkingDirectory = Path.GetDirectoryName(fileName),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = hasInput,
            UseShellExecute = false
        };

        foreach (System.Collections.DictionaryEntry variable in Environment.GetEnvironmentVariables())
            processStartInfo.Environment[(string)variable.Key] = (string?)variable.Value;

        foreach (var protocol in new[] { "http", "https" })
        {
            var name = $"{protocol}_proxy";
            var variants = new[] { name, name.ToUpperInvariant() };

            foreach (var variant in variants)
            {
                var candidate = Environment.GetEnvironmentVariable(variant);

                if (string.IsNullOrWhiteSpace(candidate))
                    continue;

                processStartInfo.Environment[variant] = candidate;

                if (!isJar)
                    continue;

                if (!candidate.Contains("://"))
                    candidate = $"{protocol}://{candidate}";

                if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
                    continue;

                arguments.Add($"-D{protocol}.proxyHost={uri.Host}");
                arguments.Add($"-D{protocol}.proxyPort={uri.Port}");

                break;
            }
        }

        if (isJar)
        {
            arguments.AddRange(jvmArguments);
            arguments.Add("-jar");
            arguments.Add(fileName);
            arguments.Add("--nogui");
            arguments.AddRange(jarArguments);
        }
        else
        {
            arguments.AddRange(userArguments);
        }

        foreach (var argument in arguments)
            processStartInfo.ArgumentList.Add(argument);

        _process = Process.Start(processStartInfo) ?? throw new IntegrationTestException($"Failed to start process for {fileName} with arguments: {string.Join(' ', arguments)}");

        _process.OutputDataReceived += OnDataReceived;
        _process.ErrorDataReceived += OnDataReceived;

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_process is not { HasExited: false })
            return;

        await _process.ExitAsync();

        _process.CancelOutputRead();
        _process.CancelErrorRead();

        _process.OutputDataReceived -= OnDataReceived;
        _process.ErrorDataReceived -= OnDataReceived;
    }

    protected async Task ReceiveOutputAsync(Func<string, bool> handler, CancellationToken cancellationToken = default)
    {
        if (_process is null)
            throw new InvalidOperationException("Application is not started.");

        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken), useSynchronizationContext: false);

        void OnProcessExited(object? sender, EventArgs eventArgs) => taskCompletionSource.TrySetCanceled(cancellationToken);
        void OnDataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Data is not null)
                {
                    if (handler(eventArgs.Data))
                        taskCompletionSource.SetResult();
                }
            }
            catch (Exception exception)
            {
                if (!taskCompletionSource.TrySetException(exception))
                    Console.WriteLine($"Failed to set exception for task completion source in {nameof(IntegrationSideBase)}.{nameof(ReceiveOutputAsync)}: {exception}");
            }
        }

        _process.OutputDataReceived += OnDataReceived;
        _process.ErrorDataReceived += OnDataReceived;
        _process.Exited += OnProcessExited;

        try
        {
            await taskCompletionSource.Task;
        }
        finally
        {
            _process.OutputDataReceived -= OnDataReceived;
            _process.ErrorDataReceived -= OnDataReceived;
            _process.Exited -= OnProcessExited;
        }
    }

    protected static async Task<string> SetupJreAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        using var disposable = await _lock.LockAsync(cancellationToken);

        var jreWorkingDirectory = Path.Combine(workingDirectory, "jre21");
        var javaExecutableName = OperatingSystem.IsWindows() ? "java.exe" : "java";
        var existingJava = Directory.Exists(jreWorkingDirectory)
            ? Directory.GetFiles(jreWorkingDirectory, javaExecutableName, SearchOption.AllDirectories).FirstOrDefault()
            : null;

        if (existingJava is null)
        {

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

            existingJava = javaPath;
        }

        return existingJava;
    }

    protected static async Task<string> GetGitHubRepositoryLatestReleaseAssetAsync(string ownerName, string repositoryName, Predicate<string> assetFilter, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = new ApiOptions
        {
            PageSize = MaxGitHubReleasesToConsider,
            PageCount = 1,
            StartPage = 1
        };

        var retries = MaxGitHubReleaseRetries;

        while (--retries > 0)
        {
            var releases = await _gitHubClient.Repository.Release.GetAll(ownerName, repositoryName, options);

            if (releases.Count is 0)
                throw new IntegrationTestException($"No releases found for {ownerName}/{repositoryName}.");

            var asset = releases
                .OrderByDescending(release => release.CreatedAt)
                .SelectMany(release => release.Assets)
                .FirstOrDefault(asset => assetFilter(asset.Name));

            if (asset is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(GitHubReleaseRetryDelaySeconds), cancellationToken);
                continue;
            }

            return asset.BrowserDownloadUrl;
        }

        throw new IntegrationTestException($"Failed to fetch releases for {ownerName}/{repositoryName} after {MaxGitHubReleaseRetries} attempts.");
    }

    private void OnDataReceived(object? sender, DataReceivedEventArgs eventArgs)
    {
        if (eventArgs.Data is not null)
            _logs.Add(eventArgs.Data);
    }
}
