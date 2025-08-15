using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

namespace Void.Tests.Integration.Sides.Servers;

public class PaperServer : IntegrationSideBase
{
    private const string ViaVersionRepositoryOwnerName = "ViaVersion";
    private const string ViaVersionRepositoryName = "ViaVersion";
    private const string ViaBackwardsRepositoryName = "ViaBackwards";
    private const string ViaRewindRepositoryName = "ViaRewind";

    private readonly string _binaryPath;

    private PaperServer(string binaryPath, string jreBinaryPath)
    {
        _binaryPath = binaryPath;
        _jreBinaryPath = jreBinaryPath;

        StartApplication(_binaryPath, hasInput: false, "-Dpaper.playerconnection.keepalive=120");
    }

    public static async Task<PaperServer> CreateAsync(string workingDirectory, HttpClient client, string instanceName = nameof(PaperServer), string? version = null, int port = 25565, PaperPlugins plugins = PaperPlugins.All, CancellationToken cancellationToken = default)
    {
        var jreBinaryPath = await SetupJreAsync(workingDirectory, client, cancellationToken);

        workingDirectory = Path.Combine(workingDirectory, instanceName);

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var versionsJson = await client.GetStringAsync("https://api.papermc.io/v2/projects/paper", cancellationToken);
        using var versions = JsonDocument.Parse(versionsJson);
        var latestVersion = versions.RootElement.GetProperty("versions").EnumerateArray().Last().GetString();
        version ??= latestVersion;

        var buildsJson = await client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{version}", cancellationToken);
        using var builds = JsonDocument.Parse(buildsJson);
        var latestBuild = builds.RootElement.GetProperty("builds").EnumerateArray().Last().GetInt32();

        var buildInfoJson = await client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{version}/builds/{latestBuild}", cancellationToken);
        using var buildInfo = JsonDocument.Parse(buildInfoJson);
        var jarName = buildInfo.RootElement.GetProperty("downloads").GetProperty("application").GetProperty("name").GetString();

        var paperUrl = $"https://api.papermc.io/v2/projects/paper/versions/{version}/builds/{latestBuild}/downloads/{jarName}";
        var paperJarPath = Path.Combine(workingDirectory, "paper.jar");

        await client.DownloadFileAsync(paperUrl, paperJarPath, cancellationToken);
        await SetupCompatibilityPluginsAsync(workingDirectory, client, plugins, cancellationToken);

        await File.WriteAllTextAsync(Path.Combine(workingDirectory, "eula.txt"), "eula=true", cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(workingDirectory, "server.properties"), $"server-port={port}\nonline-mode=false\ndifficulty=0\n", cancellationToken);

        var instance = new PaperServer(paperJarPath, jreBinaryPath);
        await instance.ExpectTextAsync("For help, type \"help\"", lookupHistory: true, cancellationToken);

        return instance;
    }

    public async Task ExpectTextAsync(string text, bool lookupHistory = true, CancellationToken cancellationToken = default)
    {
        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start Paper server.");

        if (lookupHistory && Logs.Any(log => log.Contains(text)))
            return;

        await ReceiveOutputAsync(line => HandleConsole(line, text), cancellationToken);
    }

    private static bool HandleConsole(string line, string expectedText)
    {
        if (line.Contains("java.lang.UnsupportedClassVersionError"))
            throw new IntegrationTestException("Incompatible Java version for the server");

        if (line.Contains("You need to agree to the EULA in order to run the server"))
            throw new IntegrationTestException("Server EULA not accepted");

        if (line.Contains(expectedText))
            return true;

        return false;
    }

    private static async Task SetupCompatibilityPluginsAsync(string workingDirectory, HttpClient client, PaperPlugins plugins, CancellationToken cancellationToken)
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
