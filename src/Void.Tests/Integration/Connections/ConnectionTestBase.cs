namespace Void.Tests.Integration.Connections;

using System;
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
using Void.Tests.Exceptions;
using Void.Tests.Extensions;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Servers;
using Xunit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

public abstract class ConnectionTestBase : IDisposable
{
    private const string AppName = "Void.Tests";
    private const int MaxReleasesToConsider = 3;

    private static readonly GitHubClient _gitHubClient = new(new ProductHeaderValue(AppName));
    internal static readonly string WorkingDirectory = Path.Combine(Path.GetTempPath(), AppName, "IntegrationTests");

    protected readonly HttpClient _client = new();

    private string? _jreBinaryPath;

    static ConnectionTestBase()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
            _gitHubClient.Credentials = new Credentials(token);
    }

    public ConnectionTestBase()
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");
        _client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

        if (!Directory.Exists(WorkingDirectory))
            Directory.CreateDirectory(WorkingDirectory);
    }


    public async Task ExecuteAsync(IIntegrationServer server, IIntegrationClient client, CancellationToken cancellationToken = default)
    {
        _jreBinaryPath = await SetupJreAsync(cancellationToken);

        await Task.WhenAll(server.SetupAsync(_client, cancellationToken), client.SetupAsync(_client, cancellationToken));

        Task serverTask, clientTask;

        try
        {
            serverTask = server.RunAsync(cancellationToken);
            await server.ServerLoadingTask;

            clientTask = server.RunAsync(cancellationToken);
            await serverTask;

            // Completion
            await clientTask;

            // try
            // {
            //     await clientTask;
            // }
            // catch (TaskCanceledException)
            // {
            //     // Expected when cancelling after server received expected message
            // }
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException(exception.Message + $"\nServer logs:\n{string.Join("\n", server.Logs)}\n\n\nClient logs:\n{string.Join("\n", client.Logs)}", exception);
        }
    }

    private async Task<string> SetupJreAsync(CancellationToken cancellationToken)
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

        await _client.DownloadFileAsync(url, archivePath, cancellationToken);

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

    internal static async Task<string> GetGitHubRepositoryLatestReleaseAssetAsync(string ownerName, string repositoryName, Predicate<string> assetFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = new ApiOptions
        {
            PageSize = MaxReleasesToConsider,
            PageCount = 1,
            StartPage = 1
        };

        var releases = await _gitHubClient.Repository.Release.GetAll(ownerName, repositoryName, options);

        var asset = releases
            .OrderByDescending(release => release.CreatedAt)
            .SelectMany(release => release.Assets)
            .FirstOrDefault(asset => assetFilter(asset.Name));

        Assert.NotNull(asset);

        return asset.BrowserDownloadUrl;
    }

    public void Dispose()
    {
        if (Directory.Exists(WorkingDirectory))
            Directory.Delete(WorkingDirectory, true);

        GC.SuppressFinalize(this);
    }
}
