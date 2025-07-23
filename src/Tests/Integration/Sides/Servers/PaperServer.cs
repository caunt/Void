namespace Void.Tests.Integration.Sides.Servers;

using System;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

public class PaperServer(string expectedText, PaperPlugins plugins = PaperPlugins.All) : IntegrationSideBase, IIntegrationServer
{
    private const string ViaVersionRepositoryOwnerName = "ViaVersion";
    private const string ViaVersionRepositoryName = "ViaVersion";
    private const string ViaBackwardsRepositoryName = "ViaBackwards";
    private const string ViaRewindRepositoryName = "ViaRewind";

    private string? _binaryPath;
    private TaskCompletionSource? _serverDoneTaskCompletionSource;

    public Task ServerLoadingTask => _serverDoneTaskCompletionSource?.Task ?? Task.CompletedTask;

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_binaryPath))
            throw new InvalidOperationException("Binary path is not set. Call SetupAsync first.");

        _serverDoneTaskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        cancellationToken.Register(() => _serverDoneTaskCompletionSource.TrySetCanceled(cancellationToken));

        StartApplication(_binaryPath);

        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start Paper server.");

        await HandleOutputAsync(HandleConsole, cancellationToken);
    }

    public override async Task SetupAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, "PaperServer");

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        await SetupJreAsync(workingDirectory, client, cancellationToken);

        var versionsJson = await client.GetStringAsync("https://api.papermc.io/v2/projects/paper", cancellationToken);
        using var versions = JsonDocument.Parse(versionsJson);
        var latestVersion = versions.RootElement.GetProperty("versions").EnumerateArray().Last().GetString();

        var buildsJson = await client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}", cancellationToken);
        using var builds = JsonDocument.Parse(buildsJson);
        var latestBuild = builds.RootElement.GetProperty("builds").EnumerateArray().Last().GetInt32();

        var buildInfoJson = await client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}", cancellationToken);
        using var buildInfo = JsonDocument.Parse(buildInfoJson);
        var jarName = buildInfo.RootElement.GetProperty("downloads").GetProperty("application").GetProperty("name").GetString();

        var paperUrl = $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/{jarName}";
        var paperJarPath = Path.Combine(workingDirectory, "paper.jar");

        await client.DownloadFileAsync(paperUrl, paperJarPath, cancellationToken);
        await SetupCompatibilityPluginsAsync(cancellationToken);

        await File.WriteAllTextAsync(Path.Combine(workingDirectory, "eula.txt"), "eula=true", cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(workingDirectory, "server.properties"), "server-port=25565\nonline-mode=false\n", cancellationToken);

        _binaryPath = paperJarPath;

        async Task SetupCompatibilityPluginsAsync(CancellationToken cancellationToken)
        {
            var pluginsDirectory = Path.Combine(workingDirectory, "plugins");

            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            if (plugins.HasFlag(PaperPlugins.ViaVersion))
            {
                var viaVersion = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaVersionRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
                await client.DownloadFileAsync(viaVersion, Path.Combine(pluginsDirectory, "ViaVersion.jar"), cancellationToken);
            }

            if (plugins.HasFlag(PaperPlugins.ViaBackwards))
            {
                var viaBackwards = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaBackwardsRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
                await client.DownloadFileAsync(viaBackwards, Path.Combine(pluginsDirectory, "ViaBackwards.jar"), cancellationToken);
            }

            if (plugins.HasFlag(PaperPlugins.ViaRewind))
            {
                var viaRewind = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaRewindRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
                await client.DownloadFileAsync(viaRewind, Path.Combine(pluginsDirectory, "ViaRewind.jar"), cancellationToken);
            }
        }
    }

    private bool HandleConsole(string line)
    {
        if (line.Contains("java.lang.UnsupportedClassVersionError"))
            throw new IntegrationTestException("Incompatible Java version for the server");

        if (line.Contains("You need to agree to the EULA in order to run the server"))
            throw new IntegrationTestException("Server EULA not accepted");

        if (line.Contains("Done") && line.Contains("For help, type \"help\""))
            _serverDoneTaskCompletionSource?.SetResult();

        if (line.Contains(expectedText))
            return true;

        return false;
    }

    private async Task SetupJreAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken)
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

        _jreBinaryPath = existingJava;
    }
}
