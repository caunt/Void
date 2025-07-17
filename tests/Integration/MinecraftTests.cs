using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Xunit;

namespace Void.Tests.Integration;

public class MinecraftTests : MinecraftIntegrationTestBase
{
    private const string ViaVersionRepositoryOwnerName = "ViaVersion";
    private const string ViaVersionRepositoryName = "ViaVersion";
    private const string ViaBackwardsRepositoryName = "ViaBackwards";

    private const string MinecraftConsoleClientRepositoryOwnerName = "MCCTeam";
    private const string MinecraftConsoleClientRepositoryName = "Minecraft-Console-Client";

    [Fact]
    public async Task TestAsync()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await EntryPoint.RunAsync(logWriter: logs, cancellationToken: cancellationTokenSource.Token);

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task MccConnectsToPaperServer()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        await RunAsync(cancellationTokenSource.Token);
    }

    protected override async Task<Process> StartServerAsync(CancellationToken cancellationToken)
    {
        var paperJarPath = await SetupPaperServerAsync(cancellationToken);
        return await StartApplicationAsync(paperJarPath, cancellationToken);
    }

    protected override async Task<Process> StartClientAsync(CancellationToken cancellationToken)
    {
        var clientPath = await SetupMinecraftConsoleClientAsync(ExpectedText, cancellationToken);
        return await StartApplicationAsync(clientPath, cancellationToken, "void", "-", "localhost:25565", $"send {ExpectedText}");
    }

    private async Task<string> SetupPaperServerAsync(CancellationToken cancellationToken)
    {
        var paperWorkingDirectory = Path.Combine(WorkingDirectory, "PaperServer");

        if (!Directory.Exists(paperWorkingDirectory))
            Directory.CreateDirectory(paperWorkingDirectory);

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
        var paperJarPath = Path.Combine(paperWorkingDirectory, "paper.jar");

        await DownloadFileAsync(paperUrl, paperJarPath, cancellationToken);
        await SetupCompatibilityPluginsAsync();

        await File.WriteAllTextAsync(Path.Combine(paperWorkingDirectory, "eula.txt"), "eula=true", cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(paperWorkingDirectory, "server.properties"), "server-port=25565\nonline-mode=false\n", cancellationToken);

        return paperJarPath;

        async Task SetupCompatibilityPluginsAsync()
        {
            var pluginsDirectory = Path.Combine(paperWorkingDirectory, "plugins");

            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            var viaVersion = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaVersionRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await DownloadFileAsync(viaVersion, Path.Combine(pluginsDirectory, "ViaVersion.jar"), cancellationToken);

            var viaBackwards = await GetGitHubRepositoryLatestReleaseAssetAsync(ViaVersionRepositoryOwnerName, ViaBackwardsRepositoryName, name => name.EndsWith(".jar"), cancellationToken);
            await DownloadFileAsync(viaBackwards, Path.Combine(pluginsDirectory, "ViaBackwards.jar"), cancellationToken);
        }
    }

    private async Task<string> SetupMinecraftConsoleClientAsync(string expectedText, CancellationToken cancellationToken)
    {
        var minecraftConsoleClientWorkingDirectory = Path.Combine(WorkingDirectory, "MinecraftConsoleClient");

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
            Action = "send {expectedText}"
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
}
