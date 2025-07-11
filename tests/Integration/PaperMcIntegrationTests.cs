using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Void.Tests.Integration;

public class PaperMcIntegrationTests
{
    private static async Task DownloadFileAsync(HttpClient client, string url, string destination)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        await using var fs = File.Create(destination);
        await response.Content.CopyToAsync(fs);
    }

    private static void AddProxyArgs(List<string> args, ProcessStartInfo psi)
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
        var args = new List<string>();
        var psi = new ProcessStartInfo("java")
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false
        };
        AddProxyArgs(args, psi);
        args.Add("-jar");
        args.Add(jarPath);
        args.Add("--nogui");
        foreach (var a in args)
            psi.ArgumentList.Add(a);
        return Process.Start(psi)!;
    }

    private static Process StartProcess(string fileName, string arguments, string workingDir)
    {
        var psi = new ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        foreach (var key in new[] { "HTTP_PROXY", "http_proxy", "HTTPS_PROXY", "https_proxy" })
        {
            var val = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(val))
                psi.Environment[key] = val;
        }
        return Process.Start(psi)!;
    }

    private static async Task<bool> WaitForOutputAsync(Process process, Func<string, bool> predicate, TimeSpan timeout)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        void Handler(object s, DataReceivedEventArgs e)
        {
            if (e.Data != null && predicate(e.Data))
                tcs.TrySetResult(true);
        }
        process.OutputDataReceived += Handler;
        process.ErrorDataReceived += Handler;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
        process.OutputDataReceived -= Handler;
        process.ErrorDataReceived -= Handler;
        return completed == tcs.Task && tcs.Task.Result;
    }

    private static async Task<string> GetLatestGithubAssetUrl(HttpClient client, string owner, string repo, string suffix)
    {
        var json = await client.GetStringAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
        using var doc = JsonDocument.Parse(json);
        foreach (var asset in doc.RootElement.GetProperty("assets").EnumerateArray())
        {
            var name = asset.GetProperty("name").GetString();
            if (name != null && name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return asset.GetProperty("browser_download_url").GetString()!;
        }
        throw new InvalidOperationException("No asset found");
    }

    private static string GetMccAssetSuffix()
    {
        var arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException("Unsupported architecture")
        };

        var os = OperatingSystem.IsWindows() ? "win" :
                 OperatingSystem.IsLinux() ? "linux" :
                 OperatingSystem.IsMacOS() ? "osx" :
                 throw new PlatformNotSupportedException("Unsupported OS");

        var suffix = $"{os}-{arch}";
        if (os == "win")
            suffix += ".exe";
        return suffix;
    }

    private static async Task<string> GetMccUrlAsync(HttpClient client)
    {
        var json = await client.GetStringAsync("https://api.github.com/repos/MCCTeam/Minecraft-Console-Client/releases/latest");
        using var doc = JsonDocument.Parse(json);
        var suffix = GetMccAssetSuffix();
        foreach (var asset in doc.RootElement.GetProperty("assets").EnumerateArray())
        {
            var name = asset.GetProperty("name").GetString();
            if (name != null && name.Contains(suffix, StringComparison.OrdinalIgnoreCase))
                return asset.GetProperty("browser_download_url").GetString()!;
        }
        throw new InvalidOperationException($"No {suffix} asset found");
    }

    [Fact]
    public async Task PaperServerReceivesChatMessage()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests");

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        var pluginsDir = Path.Combine(tempDir, "plugins");
        Directory.CreateDirectory(pluginsDir);
        var logs = new List<string>();
        Process server = null;
        Process mcc = null;
        try
        {
            var versionsJson = await client.GetStringAsync("https://api.papermc.io/v2/projects/paper");
            using var versions = JsonDocument.Parse(versionsJson);
            var latestVersion = versions.RootElement.GetProperty("versions").EnumerateArray().Last().GetString();
            var buildsJson = await client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}");
            using var builds = JsonDocument.Parse(buildsJson);
            var latestBuild = builds.RootElement.GetProperty("builds").EnumerateArray().Last().GetInt32();
            var buildInfoJson = await client.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}");
            using var buildInfo = JsonDocument.Parse(buildInfoJson);
            var jarName = buildInfo.RootElement.GetProperty("downloads").GetProperty("application").GetProperty("name").GetString();
            var paperUrl = $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/{jarName}";
            var paperJar = Path.Combine(tempDir, "paper.jar");
            await DownloadFileAsync(client, paperUrl, paperJar);

            var viaVersion = await GetLatestGithubAssetUrl(client, "ViaVersion", "ViaVersion", ".jar");
            await DownloadFileAsync(client, viaVersion, Path.Combine(pluginsDir, "ViaVersion.jar"));
            var viaBackwards = await GetLatestGithubAssetUrl(client, "ViaVersion", "ViaBackwards", ".jar");
            await DownloadFileAsync(client, viaBackwards, Path.Combine(pluginsDir, "ViaBackwards.jar"));

            File.WriteAllText(Path.Combine(tempDir, "eula.txt"), "eula=true");
            File.WriteAllText(Path.Combine(tempDir, "server.properties"), "server-port=25565\nonline-mode=false\n");

            server = StartJavaProcess(paperJar, tempDir);
            bool ready = await WaitForOutputAsync(server, l => l.Contains("Done") || l.Contains("Timings Reset"), TimeSpan.FromMinutes(3));
            Assert.True(ready, "Server failed to start in time");

            var mccPath = Path.Combine(tempDir, "MinecraftClient");
            var mccUrl = await GetMccUrlAsync(client);
            await DownloadFileAsync(client, mccUrl, mccPath);
            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(mccPath, UnixFileMode.UserRead | UnixFileMode.UserExecute);

            mcc = StartProcess(mccPath, "testuser - localhost:25565 \"chat hello world\"", tempDir);

            bool received = await WaitForOutputAsync(server, l => l.Contains("hello world", StringComparison.OrdinalIgnoreCase), TimeSpan.FromSeconds(30));
            Assert.True(received, "Server did not log chat message");
        }
        finally
        {
            if (mcc != null && !mcc.HasExited)
            {
                try { mcc.Kill(); } catch { }
                mcc.WaitForExit(5000);
            }
            if (server != null && !server.HasExited)
            {
                try
                {
                    await server.StandardInput.WriteLineAsync("stop");
                    server.WaitForExit(10000);
                }
                catch { }
                if (!server.HasExited)
                    server.Kill();
            }
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }
}
