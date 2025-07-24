namespace Void.Tests.Integration.Sides.Clients;

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;
using Xunit;

public class MinecraftConsoleClient : IntegrationSideBase
{
    private const string RepositoryOwnerName = "MCCTeam";
    private const string RepositoryName = "Minecraft-Console-Client";

    private readonly string _workingDirectory;
    private readonly string _binaryPath;

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion
                .Range(ProtocolVersion.MINECRAFT_1_20_3, ProtocolVersion.MINECRAFT_1_7_2)
                .Except([ProtocolVersion.MINECRAFT_1_19])]; // MCC 1.19 doesn't send chat messages

    private MinecraftConsoleClient(string workingDirectory, string binaryPath)
    {
        _workingDirectory = workingDirectory;
        _binaryPath = binaryPath;
    }

    public static async Task<MinecraftConsoleClient> CreateAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, "MinecraftConsoleClient");

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var clientFileName = OperatingSystem.IsWindows() ? "client.exe" : "client";
        var path = Path.Combine(workingDirectory, clientFileName);
        var url = await GetGitHubRepositoryLatestReleaseAssetAsync(RepositoryOwnerName, RepositoryName, name => name.EndsWith(GetMinecraftConsoleClientSuffix()), cancellationToken);

        await client.DownloadFileAsync(url, path, cancellationToken);

        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        return new(workingDirectory, path);
    }

    public async Task SendTextMessageAsync(string address, ProtocolVersion protocolVersion, string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_binaryPath))
            throw new InvalidOperationException("Binary path is not set. Call SetupAsync first.");

        await File.WriteAllTextAsync(Path.Combine(_workingDirectory, "MinecraftClient.ini"), $"""
            [Main.Advanced]
            MinecraftVersion = "{protocolVersion.MostRecentSupportedVersion}"

            [ChatBot.ScriptScheduler]
            Enabled = true

            [[ChatBot.ScriptScheduler.TaskList]]
            Task_Name = "Task Name 1"
            Trigger_On_Login = true
            Action = "send {text}"
            """, cancellationToken);

        StartApplication(_binaryPath, "void", "-", address, $"send {text}");

        var consoleTask = ReceiveOutputAsync(HandleConsole, cancellationToken);

        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start Minecraft Console Client.");

        try
        {
            await consoleTask; // Ends when HandleConsole returns true
            await Task.Delay(3_000, cancellationToken); // Since there is no way to ensure client sent the message, so just give it a few seconds to go
        }
        finally
        {
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
    }

    private bool HandleConsole(string line)
    {
        if (line.Contains("Cannot connect to the server : This version is not supported !"))
            throw new IntegrationTestException("Server / Client version mismatch");

        if (line.Contains("No connection could be made because the target machine actively refused it"))
            throw new IntegrationTestException("Server is not running or not reachable");

        if (line.Contains("Failed to check session"))
            throw new IntegrationTestException("Server is running in online mode");

        if (line.Contains("Disconnected by Server"))
            throw new IntegrationTestException("Disconnected by server - might be internal Proxy or Server exception");

        if (line.Contains("joined the game"))
            return true;

        return false;
    }

    private static string GetMinecraftConsoleClientSuffix()
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
