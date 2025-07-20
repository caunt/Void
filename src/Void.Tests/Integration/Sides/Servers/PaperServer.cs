namespace Void.Tests.Integration.Sides.Servers;

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;
using Void.Tests.Integration.Connections;

public class PaperServer(string expectedText) : IntegrationSideBase, IIntegrationServer
{
    private const string ViaVersionRepositoryOwnerName = "ViaVersion";
    private const string ViaVersionRepositoryName = "ViaVersion";
    private const string ViaBackwardsRepositoryName = "ViaBackwards";

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

    public override async Task SetupAsync(HttpClient client, CancellationToken cancellationToken = default)
    {
        var paperWorkingDirectory = Path.Combine(ConnectionTestBase.WorkingDirectory, "PaperServer");

        if (!Directory.Exists(paperWorkingDirectory))
            Directory.CreateDirectory(paperWorkingDirectory);

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
        var paperJarPath = Path.Combine(paperWorkingDirectory, "paper.jar");

        await client.DownloadFileAsync(paperUrl, paperJarPath, cancellationToken);
        await SetupCompatibilityPluginsAsync(cancellationToken);

        await File.WriteAllTextAsync(Path.Combine(paperWorkingDirectory, "eula.txt"), "eula=true", cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(paperWorkingDirectory, "server.properties"), "server-port=25565\nonline-mode=false\n", cancellationToken);

        _binaryPath = paperJarPath;

        async Task SetupCompatibilityPluginsAsync(CancellationToken cancellationToken)
        {
            var pluginsDirectory = Path.Combine(ConnectionTestBase.WorkingDirectory, "PaperServer", "plugins");

            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            var viaVersion = await ConnectionTestBase.GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaVersionRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await client.DownloadFileAsync(viaVersion, Path.Combine(pluginsDirectory, "ViaVersion.jar"), cancellationToken);

            var viaBackwards = await ConnectionTestBase.GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaBackwardsRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await client.DownloadFileAsync(viaBackwards, Path.Combine(pluginsDirectory, "ViaBackwards.jar"), cancellationToken);
        }
    }

    private bool HandleConsole(string line)
    {
        if (line.Contains("java.lang.UnsupportedClassVersionError"))
            throw new IntegrationTestException("Incompatible Java version for the server");

        if (line.Contains("Done") && line.Contains("For help, type \"help\""))
            _serverDoneTaskCompletionSource?.SetResult();

        if (line.Contains(expectedText))
            return true;

        return false;
    }
}
