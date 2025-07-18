namespace Void.Tests.Integration;

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Integration.Interfaces;

internal class MinecraftConsoleClient : IIntegrationClient
{
    private const string RepositoryOwnerName = "MCCTeam";
    private const string RepositoryName = "Minecraft-Console-Client";
    private readonly string _expectedText;

    public MinecraftConsoleClient(string expectedText)
    {
        _expectedText = expectedText;
    }

    public async Task<Process> StartAsync(ConnectionTestBase test, CancellationToken cancellationToken)
    {
        var clientPath = await SetupMinecraftConsoleClientAsync(test, cancellationToken);
        return await test.StartApplicationAsync(clientPath, cancellationToken, "void", "-", "localhost:25565", $"send {_expectedText}");
    }

    private async Task<string> SetupMinecraftConsoleClientAsync(ConnectionTestBase test, CancellationToken cancellationToken)
    {
        var workingDirectory = Path.Combine(ConnectionTestBase.WorkingDirectory, "MinecraftConsoleClient");

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var path = Path.Combine(workingDirectory, "client");
        var url = await ConnectionTestBase.GetGitHubRepositoryLatestReleaseAssetAsync(RepositoryOwnerName, RepositoryName, name => name.EndsWith(GetMinecraftConsoleClientSuffix()), cancellationToken);

        await test.DownloadFileAsync(url, path, cancellationToken);

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
            Action = "send {_expectedText}"
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
