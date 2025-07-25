using System;
using System.IO;
using System.Net.Http;
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

    private readonly string _launcherPath;

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion
        .Range(ProtocolVersion.MINECRAFT_1_21_5, ProtocolVersion.MINECRAFT_1_7_2)
    ];

    private HeadlessMcClient(string launcherPath, string jreBinaryPath)
    {
        _launcherPath = launcherPath;
        _jreBinaryPath = jreBinaryPath;
    }

    public static async Task<HeadlessMcClient> CreateAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        var jreBinaryPath = await SetupJreAsync(workingDirectory, client, cancellationToken);

        workingDirectory = Path.Combine(workingDirectory, nameof(HeadlessMcClient));

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var url = await GetGitHubRepositoryLatestReleaseAssetAsync(RepositoryOwnerName, RepositoryName, name => name.StartsWith("headlessmc-launcher") && name.EndsWith(".jar"), cancellationToken);

        var launcherPath = Path.Combine(workingDirectory, "headlessmc-launcher.jar");
        await client.DownloadFileAsync(url, launcherPath, cancellationToken);

        return new HeadlessMcClient(launcherPath, jreBinaryPath);
    }

    public async Task SendTextMessageAsync(string address, ProtocolVersion protocolVersion, string text, CancellationToken cancellationToken = default)
    {
        StartApplication(_launcherPath, hasInput: true,
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
            await WriteInputAsync($"msg {text}", cancellationToken);

            await Task.Delay(5000, cancellationToken);

            await WriteInputAsync("quit", cancellationToken);
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
        finally
        {
            if (_process is { HasExited: false })
                await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
    }

    private async Task WriteInputAsync(string command, CancellationToken cancellationToken)
    {
        if (_process?.StandardInput is null)
            throw new IntegrationTestException("Process standard input is not available");

        await _process.StandardInput.WriteLineAsync(command.AsMemory(), cancellationToken);
        await _process.StandardInput.FlushAsync(cancellationToken);
    }
}
