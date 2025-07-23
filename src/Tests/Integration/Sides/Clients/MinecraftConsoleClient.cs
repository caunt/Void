namespace Void.Tests.Integration.Sides.Clients;

using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

public class MinecraftConsoleClient(string sendText, string address) : IntegrationSideBase, IIntegrationClient
{
    private const string RepositoryOwnerName = "MCCTeam";
    private const string RepositoryName = "Minecraft-Console-Client";

    private string? _binaryPath;

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_binaryPath))
            throw new InvalidOperationException("Binary path is not set. Call SetupAsync first.");

        StartApplication(_binaryPath, "void", "-", address, $"send {sendText}");

        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start Minecraft Console Client.");

        await HandleOutputAsync(HandleConsole, cancellationToken);
    }

    public override async Task SetupAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, "MinecraftConsoleClient");

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var path = Path.Combine(workingDirectory, "client");
        var url = await GetGitHubRepositoryLatestReleaseAssetAsync(RepositoryOwnerName, RepositoryName, name => name.EndsWith(GetMinecraftConsoleClientSuffix()), cancellationToken);

        await client.DownloadFileAsync(url, path, cancellationToken);

        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        await File.WriteAllTextAsync(Path.Combine(workingDirectory, "MinecraftClient.ini"), $"""
            [Main.Advanced]
            MinecraftVersion = "1.20.4"

            [ChatBot.ScriptScheduler]
            Enabled = true

            [[ChatBot.ScriptScheduler.TaskList]]
            Task_Name = "Task Name 1"
            Trigger_On_Login = true
            Action = "send {sendText}"
            """, cancellationToken);

        _binaryPath = path;

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

    private bool HandleConsole(string line)
    {
        if (line.Contains("Cannot connect to the server : This version is not supported !"))
            throw new IntegrationTestException("Server / Client version mismatch");

        if (line.Contains("No connection could be made because the target machine actively refused it"))
            throw new IntegrationTestException("Server is not running or not reachable");

        if (line.Contains("Failed to check session"))
            throw new IntegrationTestException("Server is running in online mode");

        return false;
    }
}
