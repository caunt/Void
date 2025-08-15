namespace Void.Tests.Integration.Sides.Clients;

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
using Void.Minecraft.Network;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;
using Xunit;

public class MineflayerClient : IntegrationSideBase
{
    private readonly string _nodePath;
    private readonly string _scriptPath;

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion
                .Range(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_8)];

    private MineflayerClient(string nodePath, string scriptPath)
    {
        _nodePath = nodePath;
        _scriptPath = scriptPath;
    }

    public static async Task<MineflayerClient> CreateAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, nameof(MineflayerClient));

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var nodePath = await SetupNodeAsync(workingDirectory, client, cancellationToken);
        await InstallMineflayerAsync(nodePath, workingDirectory, cancellationToken);

        var scriptPath = Path.Combine(workingDirectory, "bot.js");
        await File.WriteAllTextAsync(scriptPath, $$"""
            const mineflayer = require('mineflayer');
            const [address, version, text] = process.argv.slice(2);
            const [host, portString] = address.split(':');
            const port = parseInt(portString ?? '25565', 10);
            const bot = mineflayer.createBot({ host, port, username: '{{nameof(MineflayerClient)}}', version });

            bot.on('spawn', () => {
                bot.chat(text);
                setTimeout(() => {
                    console.log('end');
                    bot.end();
                }, 5000);
            });

            bot.on('kicked', reason => console.error('KICK:' + reason));
            bot.on('error', err => console.error('ERROR:' + err.message));
            """, cancellationToken);

        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(scriptPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        return new(nodePath, scriptPath);
    }

    public async Task SendTextMessageAsync(string address, ProtocolVersion protocolVersion, string text, CancellationToken cancellationToken = default)
    {
        StartApplication(_nodePath, hasInput: false, _scriptPath, address, protocolVersion.MostRecentSupportedVersion, text);

        var consoleTask = ReceiveOutputAsync(HandleConsole, cancellationToken);

        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start Mineflayer bot.");

        try
        {
            await consoleTask;
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
        finally
        {
            if (_process is { HasExited: false })
                await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
    }

    public async Task SwitchServersAsync(string address, ProtocolVersion protocolVersion, string[] serverNames, string[] messages, CancellationToken cancellationToken = default)
    {
        if (serverNames.Length != messages.Length)
            throw new ArgumentException("Server names and messages length must match");

        var workingDirectory = Path.GetDirectoryName(_scriptPath) ?? throw new IntegrationTestException("Invalid script path");
        var scriptPath = Path.Combine(workingDirectory, "switch-server.js");

        var steps = string.Join(",", serverNames.Zip(messages, (s, m) => $"{{server: '{s}', message: '{m}'}}"));

        await File.WriteAllTextAsync(scriptPath, $$"""
            const mineflayer = require('mineflayer');
            const [address, version] = process.argv.slice(2);
            const [host, portString] = address.split(':');
            const port = parseInt(portString ?? '25565', 10);
            const steps = [{{steps}}];
            const bot = mineflayer.createBot({ host, port, username: '{{nameof(MineflayerClient)}}', version });
            let index = 0;
            function executeStep() {
                const step = steps[index];
                bot.chat(step.message);
                index++;
                if (index < steps.length) {
                    setTimeout(() => {
                        bot.chat('/server ' + steps[index].server);
                        setTimeout(executeStep, 8000);
                    }, 1000);
                } else {
                    setTimeout(() => {
                        console.log('end');
                        bot.end();
                    }, 5000);
                }
            }
            bot.once('spawn', () => {
                setTimeout(executeStep, 2000);
            });
            bot.on('kicked', reason => console.error('KICK:' + reason));
            bot.on('error', err => console.error('ERROR:' + err.message));
            """, cancellationToken);

        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(scriptPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        StartApplication(_nodePath, hasInput: false, scriptPath, address, protocolVersion.MostRecentSupportedVersion);

        var consoleTask = ReceiveOutputAsync(HandleConsole, cancellationToken);

        if (_process is not { HasExited: false })
            throw new IntegrationTestException("Failed to start Mineflayer bot.");

        try
        {
            await consoleTask;
            await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
        finally
        {
            if (_process is { HasExited: false })
                await _process.ExitAsync(entireProcessTree: true, cancellationToken);
        }
    }

    private static bool HandleConsole(string line)
    {
        if (line.StartsWith("ERROR:"))
            throw new IntegrationTestException(line);

        if (line.StartsWith("KICK:"))
            throw new IntegrationTestException(line);

        if (line.Contains("end"))
            return true;

        return false;
    }

    private static async Task<string> SetupNodeAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken)
    {
        var nodeDirectory = Path.Combine(workingDirectory, "node");
        var nodeExecutableName = OperatingSystem.IsWindows() ? "node.exe" : "node";

        var existingNode = Directory.Exists(nodeDirectory)
            ? Directory.GetFiles(nodeDirectory, nodeExecutableName, SearchOption.AllDirectories).FirstOrDefault()
            : null;

        if (existingNode is null)
        {
            if (!Directory.Exists(nodeDirectory))
                Directory.CreateDirectory(nodeDirectory);

            var indexJson = await client.GetStringAsync("https://nodejs.org/dist/index.json", cancellationToken);
            using var index = JsonDocument.Parse(indexJson);

            var ltsVersion = index.RootElement.EnumerateArray()
                .First(element => element.TryGetProperty("lts", out var lts) && lts.ValueKind != JsonValueKind.False)
                .GetProperty("version").GetString();

            var os = OperatingSystem.IsWindows() ? "win" : OperatingSystem.IsLinux() ? "linux" : OperatingSystem.IsMacOS() ? "darwin" : throw new PlatformNotSupportedException("Unsupported OS");
            var arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm64 => "arm64",
                Architecture.Arm => OperatingSystem.IsWindows() ? "arm64" : "armv7l",
                _ => throw new PlatformNotSupportedException("Unsupported architecture")
            };

            var extension = OperatingSystem.IsWindows() ? "zip" : "tar.gz";
            var url = $"https://nodejs.org/dist/{ltsVersion}/node-{ltsVersion}-{os}-{arch}.{extension}";
            var archivePath = Path.Combine(nodeDirectory, Path.GetFileName(url));

            await client.DownloadFileAsync(url, archivePath, cancellationToken);

            if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                ZipFile.ExtractToDirectory(archivePath, nodeDirectory);
            }
            else
            {
                await using var fileStream = File.OpenRead(archivePath);
                using var gzip = new GZipStream(fileStream, CompressionMode.Decompress);
                TarFile.ExtractToDirectory(gzip, nodeDirectory, overwriteFiles: true);
            }

            existingNode = Directory.GetFiles(nodeDirectory, nodeExecutableName, SearchOption.AllDirectories).FirstOrDefault() ??
                throw new IntegrationTestException("Failed to locate downloaded Node binary");

            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(existingNode, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        }

        return existingNode;
    }

    private static async Task InstallMineflayerAsync(string nodePath, string workingDirectory, CancellationToken cancellationToken)
    {
        var nodeRoot = Path.GetDirectoryName(nodePath) ?? throw new IntegrationTestException("Invalid Node path");

        if (nodeRoot.EndsWith("bin"))
            nodeRoot = Path.GetDirectoryName(nodeRoot) ?? throw new IntegrationTestException("Invalid Node path");

        var npmCli = Path.Combine(nodeRoot, "lib", "node_modules", "npm", "bin", "npm-cli.js");

        if (!File.Exists(npmCli))
            npmCli = Path.Combine(nodeRoot, "node_modules", "npm", "bin", "npm-cli.js");

        await RunNpmAsync(npmCli, "init", "-y");
        await RunNpmAsync(npmCli, "install", "mineflayer");

        async Task RunNpmAsync(params string[] arguments)
        {
            var startInfo = new ProcessStartInfo(nodePath)
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var argument in arguments)
                startInfo.ArgumentList.Add(argument);

            using var process = Process.Start(startInfo) ?? throw new IntegrationTestException($"Failed to start NPM at {nodePath}");

            var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var stdOut = await stdOutTask;
            var stdErr = await stdErrTask;

            if (process.ExitCode != 0)
            {
                var logs = $"STDOUT:\n{stdOut}\nSTDERR:\n{stdErr}";
                throw new IntegrationTestException($"NPM at {nodePath} exited with code {process.ExitCode}\n{logs}");
            }
        }
    }
}
