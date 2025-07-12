using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Void.Tests.Integration;

public class MinecraftTests : IDisposable
{
    private static readonly string _workingDirectory = Path.Combine(Path.GetTempPath(), "Void.Tests", "PaperMcIntegrationTests");
    private static readonly string _pluginsDirectory = Path.Combine(_workingDirectory, "plugins");

    private readonly HttpClient _client = new();

    public MinecraftTests()
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");

        // Disable caching to ensure we always get the latest data
        _client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true
        };

        if (!Directory.Exists(_workingDirectory))
            Directory.CreateDirectory(_workingDirectory);

        if (!Directory.Exists(_pluginsDirectory))
            Directory.CreateDirectory(_pluginsDirectory);
    }

    [Fact]
    public async Task MccConnectsPaperServer()
    {
        var logs = new List<string>();
        Process server = null;
        Process mcc = null;

        try
        {
            var versionsJson = await _client.GetStringAsync("https://api.papermc.io/v2/projects/paper");
            using var versions = JsonDocument.Parse(versionsJson);
            var latestVersion = versions.RootElement.GetProperty("versions").EnumerateArray().Last().GetString();

            var buildsJson = await _client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}");
            using var builds = JsonDocument.Parse(buildsJson);
            var latestBuild = builds.RootElement.GetProperty("builds").EnumerateArray().Last().GetInt32();

            var buildInfoJson = await _client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}");
            using var buildInfo = JsonDocument.Parse(buildInfoJson);
            var jarName = buildInfo.RootElement.GetProperty("downloads").GetProperty("application").GetProperty("name").GetString();

            var paperUrl = $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/{jarName}";
            var paperJar = Path.Combine(_workingDirectory, "paper.jar");

            await DownloadFileAsync(paperUrl, paperJar);

            var viaVersion = await GetLatestGithubAssetUrl("ViaVersion", "ViaVersion", ".jar");
            await DownloadFileAsync(viaVersion, Path.Combine(_pluginsDirectory, "ViaVersion.jar"));

            var viaBackwards = await GetLatestGithubAssetUrl("ViaVersion", "ViaBackwards", ".jar");
            await DownloadFileAsync(viaBackwards, Path.Combine(_pluginsDirectory, "ViaBackwards.jar"));

            File.WriteAllText(Path.Combine(_workingDirectory, "eula.txt"), "eula=true");
            File.WriteAllText(Path.Combine(_workingDirectory, "server.properties"), "server-port=25565\nonline-mode=false\n");

            server = StartJavaProcess(paperJar, _workingDirectory);
            bool ready = await WaitForOutputAsync(server, l => l.Contains("Done") || l.Contains("Timings Reset"), TimeSpan.FromMinutes(3));
            Assert.True(ready, "Server failed to start in time");

            var mccPath = Path.Combine(_workingDirectory, "MinecraftClient");
            var mccUrl = await GetMccUrlAsync();

            await DownloadFileAsync(mccUrl, mccPath);

            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(mccPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

            mcc = StartProcess(mccPath, "testuser - localhost:25565 \"chat hello world\"", _workingDirectory);

            bool received = await WaitForOutputAsync(server, l => l.Contains("hello world", StringComparison.OrdinalIgnoreCase), TimeSpan.FromSeconds(30));
            Assert.True(received, "Server did not log chat message");
        }
        finally
        {
            if (mcc != null && !mcc.HasExited)
            {
                mcc.Kill();
                mcc.WaitForExit(5000);
            }
            if (server != null && !server.HasExited)
            {
                await server.StandardInput.WriteLineAsync("stop");
                server.WaitForExit(10000);

                if (!server.HasExited)
                    server.Kill();
            }
        }
    }

    public void Dispose()
    {
        Directory.Delete(_pluginsDirectory, true);
        Directory.Delete(_workingDirectory, true);

        GC.SuppressFinalize(this);
    }

    private async Task DownloadFileAsync(string url, string destination)
    {
        using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(destination);
        await response.Content.CopyToAsync(fileStream);
    }

    private static void AddProxyArguments(List<string> args, ProcessStartInfo psi)
    {
        foreach (var key in new[] { "HTTP_PROXY", "http_proxy" })
        {
            var value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrWhiteSpace(value))
                continue;

            psi.Environment[key] = value;

            if (!value.Contains("://"))
                value = "http://" + value;

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                args.Add($"-Dhttp.proxyHost={uri.Host}");
                args.Add($"-Dhttp.proxyPort={uri.Port}");

                break;
            }
        }

        foreach (var key in new[] { "HTTPS_PROXY", "https_proxy" })
        {
            var value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrWhiteSpace(value))
                continue;

            psi.Environment[key] = value;

            if (!value.Contains("://"))
                value = "http://" + value;

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                args.Add($"-Dhttps.proxyHost={uri.Host}");
                args.Add($"-Dhttps.proxyPort={uri.Port}");

                break;
            }
        }
    }

    private static Process StartJavaProcess(string jarPath, string workingDir)
    {
        var arguments = new List<string>();
        var processStartInfo = new ProcessStartInfo("java")
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false
        };

        AddProxyArguments(arguments, processStartInfo);

        arguments.Add("-jar");
        arguments.Add(jarPath);
        arguments.Add("--nogui");

        foreach (var argument in arguments)
            processStartInfo.ArgumentList.Add(argument);

        return Process.Start(processStartInfo);
    }

    private static Process StartProcess(string fileName, string arguments, string workingDir)
    {
        var processStartInfo = new ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        foreach (var environmentVariableName in new[] { "HTTP_PROXY", "http_proxy", "HTTPS_PROXY", "https_proxy" })
        {
            var environmentVariableValue = Environment.GetEnvironmentVariable(environmentVariableName);

            if (!string.IsNullOrWhiteSpace(environmentVariableValue))
                processStartInfo.Environment[environmentVariableName] = environmentVariableValue;
        }

        return Process.Start(processStartInfo);
    }

    private static async Task<bool> WaitForOutputAsync(Process process, Func<string, bool> predicate, TimeSpan timeout)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void Handler(object sender, DataReceivedEventArgs eventArgs)
        {
            if (eventArgs.Data != null && predicate(eventArgs.Data))
                taskCompletionSource.TrySetResult(true);
        }

        process.OutputDataReceived += Handler;
        process.ErrorDataReceived += Handler;

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var completed = await Task.WhenAny(taskCompletionSource.Task, Task.Delay(timeout));

        process.CancelOutputRead();
        process.CancelErrorRead();

        process.OutputDataReceived -= Handler;
        process.ErrorDataReceived -= Handler;

        return completed == taskCompletionSource.Task && taskCompletionSource.Task.Result;
    }

    private async Task<string> GetLatestGithubAssetUrl(string owner, string repo, string suffix)
    {
        var json = await _client.GetStringAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
        using var document = JsonDocument.Parse(json);

        foreach (var asset in document.RootElement.GetProperty("assets").EnumerateArray())
        {
            var name = asset.GetProperty("name").GetString();

            if (name != null && name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return asset.GetProperty("browser_download_url").GetString();
        }

        throw new InvalidOperationException("No asset found");
    }

    private static string GetMccAssetSuffix()
    {
        var architecture = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException("Unsupported architecture")
        };

        var operatingSystem = OperatingSystem.IsWindows() ? "win" :
                 OperatingSystem.IsLinux() ? "linux" :
                 OperatingSystem.IsMacOS() ? "osx" :
                 throw new PlatformNotSupportedException("Unsupported OS");

        var suffix = $"{operatingSystem}-{architecture}";

        if (operatingSystem == "win")
            suffix += ".exe";

        return suffix;
    }

    private async Task<string> GetMccUrlAsync()
    {
        var json = await _client.GetStringAsync("https://api.github.com/repos/MCCTeam/Minecraft-Console-Client/releases/latest");
        using var document = JsonDocument.Parse(json);

        var suffix = GetMccAssetSuffix();

        foreach (var asset in document.RootElement.GetProperty("assets").EnumerateArray())
        {
            var name = asset.GetProperty("name").GetString();

            if (name != null && name.Contains(suffix, StringComparison.OrdinalIgnoreCase))
                return asset.GetProperty("browser_download_url").GetString();
        }

        throw new InvalidOperationException($"No {suffix} asset found");
    }
}
