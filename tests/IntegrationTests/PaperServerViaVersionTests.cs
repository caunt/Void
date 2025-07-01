using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Void.Tests;

public class PaperServerViaVersionTests
{
    [Fact]
    public async Task PaperServerAllowsOldClientAsync()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("VoidTest/1.0");

        var versions = await client.GetFromJsonAsync<PaperProject>("https://api.papermc.io/v2/projects/paper");
        var latestVersion = versions!.versions.Last();
        var builds = await client.GetFromJsonAsync<PaperBuilds>($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}");
        var latestBuild = builds!.builds.Last();
        var jarUrl = $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/paper-{latestVersion}-{latestBuild}.jar";

        var dir = Path.Combine(Path.GetTempPath(), $"paper_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        var pluginsDir = Path.Combine(dir, "plugins");
        Directory.CreateDirectory(pluginsDir);

        async Task DownloadAsync(string url, string path)
        {
            using var stream = await client.GetStreamAsync(url);
            await using var fs = File.Create(path);
            await stream.CopyToAsync(fs);
        }

        var serverJar = Path.Combine(dir, "paper.jar");
        await DownloadAsync(jarUrl, serverJar);

        var viaVersionJar = Path.Combine(pluginsDir, "ViaVersion.jar");
        await DownloadAsync("https://api.spiget.org/v2/resources/19254/download", viaVersionJar);

        await File.WriteAllTextAsync(Path.Combine(dir, "eula.txt"), "eula=true\n");
        await File.WriteAllTextAsync(Path.Combine(dir, "server.properties"), "online-mode=false\n");

        using var server = StartProcess(
            "java",
            $"-Djava.net.preferIPv4Stack=true -jar {serverJar} --nogui",
            dir);
        try
        {
            await WaitForOutputAsync(server, "Done", TimeSpan.FromMinutes(2));

            var release = await client.GetFromJsonAsync<GithubRelease>("https://api.github.com/repos/MCCTeam/Minecraft-Console-Client/releases/latest");
            var asset = release!.assets.First(a => a.name.Contains("linux-x64"));
            var mccPath = Path.Combine(dir, "MCC");
            await DownloadAsync(asset.browser_download_url, mccPath);
            Process.Start("chmod", $"+x {mccPath}")!.WaitForExit();

            using var mcc = StartProcess(mccPath, $"test - 127.0.0.1:25565 \"/send hello world\"", dir);
            await WaitForOutputAsync(server, "hello world", TimeSpan.FromSeconds(30));
        }
        finally
        {
            if (!server.HasExited)
            {
                server.Kill();
                server.WaitForExit(10000);
            }

            try
            {
                Directory.Delete(dir, true);
            }
            catch
            {
                // ignore cleanup failures
            }
        }
    }

    private static Process StartProcess(string file, string args, string working)
    {
        var psi = new ProcessStartInfo(file, args)
        {
            WorkingDirectory = working,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        void SetEnv(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
                psi.EnvironmentVariables[name] = value;
        }

        SetEnv("HTTP_PROXY");
        SetEnv("HTTPS_PROXY");
        SetEnv("NO_PROXY");
        SetEnv("http_proxy");
        SetEnv("https_proxy");
        SetEnv("no_proxy");

        var process = Process.Start(psi)!;
        Task.Run(async () =>
        {
            while (!process.HasExited)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (line == null)
                    break;
                Trace.WriteLine(line);
            }
        });
        return process;
    }

    private static async Task WaitForOutputAsync(Process process, string text, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        while (!cts.IsCancellationRequested)
        {
            if (process.HasExited)
                throw new Exception("Process exited before expected output");
            var line = await process.StandardOutput.ReadLineAsync();
            if (line != null)
            {
                Trace.WriteLine(line);
                if (line.Contains(text, StringComparison.OrdinalIgnoreCase))
                    return;
            }
        }
        throw new TimeoutException($"Did not see '{text}' in output within {timeout}");
    }

    private sealed record PaperProject(string[] versions);
    private sealed record PaperBuilds(int[] builds);
    private sealed record GithubRelease(GithubAsset[] assets);
    private sealed record GithubAsset(string name, [property: JsonPropertyName("browser_download_url")] string browser_download_url);
}
