namespace Void.Tests.Integration;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Integration.Interfaces;

internal class PaperServer : IIntegrationServer
{
    private const string ViaVersionRepositoryOwnerName = "ViaVersion";
    private const string ViaVersionRepositoryName = "ViaVersion";
    private const string ViaBackwardsRepositoryName = "ViaBackwards";

    public async Task<Process> StartAsync(ConnectionTestBase test, CancellationToken cancellationToken)
    {
        var paperJarPath = await SetupPaperServerAsync(test, cancellationToken);
        return await test.StartApplicationAsync(paperJarPath, cancellationToken);
    }

    private static async Task<string> SetupPaperServerAsync(ConnectionTestBase test, CancellationToken cancellationToken)
    {
        var paperWorkingDirectory = Path.Combine(ConnectionTestBase.WorkingDirectory, "PaperServer");

        if (!Directory.Exists(paperWorkingDirectory))
            Directory.CreateDirectory(paperWorkingDirectory);

        var versionsJson = await test.Client.GetStringAsync("https://api.papermc.io/v2/projects/paper", cancellationToken);
        using var versions = JsonDocument.Parse(versionsJson);
        var latestVersion = versions.RootElement.GetProperty("versions").EnumerateArray().Last().GetString();

        var buildsJson = await test.Client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}", cancellationToken);
        using var builds = JsonDocument.Parse(buildsJson);
        var latestBuild = builds.RootElement.GetProperty("builds").EnumerateArray().Last().GetInt32();

        var buildInfoJson = await test.Client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}", cancellationToken);
        using var buildInfo = JsonDocument.Parse(buildInfoJson);
        var jarName = buildInfo.RootElement.GetProperty("downloads").GetProperty("application").GetProperty("name").GetString();

        var paperUrl = $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/{jarName}";
        var paperJarPath = Path.Combine(paperWorkingDirectory, "paper.jar");

        await test.DownloadFileAsync(paperUrl, paperJarPath, cancellationToken);
        await SetupCompatibilityPluginsAsync(test, cancellationToken);

        await File.WriteAllTextAsync(Path.Combine(paperWorkingDirectory, "eula.txt"), "eula=true", cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(paperWorkingDirectory, "server.properties"), "server-port=25565\nonline-mode=false\n", cancellationToken);

        return paperJarPath;

        static async Task SetupCompatibilityPluginsAsync(ConnectionTestBase test, CancellationToken cancellationToken)
        {
            var pluginsDirectory = Path.Combine(ConnectionTestBase.WorkingDirectory, "PaperServer", "plugins");

            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            var viaVersion = await ConnectionTestBase.GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaVersionRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await test.DownloadFileAsync(viaVersion, Path.Combine(pluginsDirectory, "ViaVersion.jar"), cancellationToken);

            var viaBackwards = await ConnectionTestBase.GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaBackwardsRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await test.DownloadFileAsync(viaBackwards, Path.Combine(pluginsDirectory, "ViaBackwards.jar"), cancellationToken);
        }
    }
}
