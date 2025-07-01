using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
#nullable enable
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

        await using var dir = new TempDir();
        var pluginsDir = Path.Combine(dir.Path, "plugins");
        Directory.CreateDirectory(pluginsDir);

        var serverJar = Path.Combine(dir.Path, "paper.jar");
        await DownloadFileAsync(client, await GetLatestPaperJarUrlAsync(client), serverJar);

        await DownloadGithubReleaseAssetAsync(client, "ViaVersion/ViaVersion", a => a.name.EndsWith(".jar", StringComparison.OrdinalIgnoreCase), Path.Combine(pluginsDir, "ViaVersion.jar"));
        await DownloadGithubReleaseAssetAsync(client, "ViaVersion/ViaBackwards", a => a.name.EndsWith(".jar", StringComparison.OrdinalIgnoreCase), Path.Combine(pluginsDir, "ViaBackwards.jar"));

        await File.WriteAllTextAsync(Path.Combine(dir.Path, "eula.txt"), "eula=true\n");
        await File.WriteAllTextAsync(Path.Combine(dir.Path, "server.properties"), "online-mode=false\n");

        var output = new ConcurrentQueue<string>();
        using var server = StartProcess(
            "java",
            $"-Djava.net.preferIPv4Stack=true {GetJavaProxyArgs()} -jar {serverJar} --nogui",
            dir.Path,
            output);
        try
        {
            await WaitForOutputAsync(server, output, "Done (", TimeSpan.FromMinutes(3));

            var release = await client.GetFromJsonAsync<GithubRelease>("https://api.github.com/repos/MCCTeam/Minecraft-Console-Client/releases/latest");
            var asset = release!.assets.First(a => a.name.Contains("linux-x64"));
            var mccPath = Path.Combine(dir.Path, "MCC");
            await DownloadFileAsync(client, asset.browser_download_url, mccPath);
            Process.Start("chmod", $"+x {mccPath}")!.WaitForExit();

            using var mcc = StartProcess(mccPath, $"test - 127.0.0.1:25565 \"hello world\"", dir.Path, output);
            await WaitForOutputAsync(server, output, "<test> hello world", TimeSpan.FromSeconds(30));
        }
        finally
        {
            if (!server.HasExited)
            {
                server.Kill();
                server.WaitForExit(10000);
            }
        }
    }

    private static Process StartProcess(string file, string args, string working, ConcurrentQueue<string> output)
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
                output.Enqueue(line);
                Trace.WriteLine(line);
            }
        });
        Task.Run(async () =>
        {
            while (!process.HasExited)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (line == null)
                    break;
                output.Enqueue(line);
                Trace.WriteLine(line);
            }
        });
        return process;
    }

    private static async Task WaitForOutputAsync(Process process, ConcurrentQueue<string> output, string text, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        while (!cts.IsCancellationRequested)
        {
            while (output.TryDequeue(out var line))
            {
                if (line.Contains(text, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            if (process.HasExited)
                throw new Exception("Process exited before expected output");

            await Task.Delay(100, cts.Token);
        }
        throw new TimeoutException($"Did not see '{text}' in output within {timeout}");
    }

    private static string GetJavaProxyArgs()
    {
        static string? GetProxyEnv()
        {
            foreach (var name in new[] { "HTTPS_PROXY", "https_proxy", "HTTP_PROXY", "http_proxy" })
            {
                var value = Environment.GetEnvironmentVariable(name);
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return null;
        }

        var proxy = GetProxyEnv();
        if (proxy is null)
            return string.Empty;

        if (!Uri.TryCreate(proxy, UriKind.Absolute, out var uri))
            return string.Empty;

        return $"-Dhttp.proxyHost={uri.Host} -Dhttp.proxyPort={uri.Port} -Dhttps.proxyHost={uri.Host} -Dhttps.proxyPort={uri.Port}";
    }

    private static async Task<string> GetLatestPaperJarUrlAsync(HttpClient client)
    {
        var versions = await client.GetFromJsonAsync<PaperProject>("https://api.papermc.io/v2/projects/paper");
        var latestVersion = versions!.versions.Last();
        var builds = await client.GetFromJsonAsync<PaperBuilds>($"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}");
        var latestBuild = builds!.builds.Last();
        return $"https://api.papermc.io/v2/projects/paper/versions/{latestVersion}/builds/{latestBuild}/downloads/paper-{latestVersion}-{latestBuild}.jar";
    }

    private static async Task DownloadGithubReleaseAssetAsync(HttpClient client, string repo, Func<GithubAsset, bool> match, string path)
    {
        var release = await client.GetFromJsonAsync<GithubRelease>($"https://api.github.com/repos/{repo}/releases/latest");
        var assetUrl = release!.assets.First(match).browser_download_url;
        await DownloadFileAsync(client, assetUrl, path);
    }

    private static async Task DownloadFileAsync(HttpClient client, string url, string path)
    {
        await using var fs = File.Create(path);
        using var stream = await client.GetStreamAsync(url);
        await stream.CopyToAsync(fs);
    }

    private sealed class TempDir : IAsyncDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "paper_test_" + Guid.NewGuid().ToString("N"));

        public TempDir() => Directory.CreateDirectory(Path);

        public ValueTask DisposeAsync()
        {
            try
            {
                Directory.Delete(Path, true);
            }
            catch
            {
                // ignore cleanup failures
            }

            return ValueTask.CompletedTask;
        }
    }

    private sealed record PaperProject(string[] versions);
    private sealed record PaperBuilds(int[] builds);
    private sealed record GithubRelease(GithubAsset[] assets);
    private sealed record GithubAsset(string name, [property: JsonPropertyName("browser_download_url")] string browser_download_url);
}
