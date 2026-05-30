#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net11.0
#:property PublishAot=false
#:package CurseForge.APIClient@*

using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using CurseForge.APIClient;

var builder = WebApplication.CreateBuilder(args);
var application = builder.Build();
var clientProcess = (Process?)null;
var clientLock = new SemaphoreSlim(1, 1);

application.MapGet("/health", () => "ok");

application.MapPost("/start-vanilla", async (HttpContext context) =>
{
    var request = await context.Request.ReadFromJsonAsync<VanillaRequest>();
    if (request is null || string.IsNullOrWhiteSpace(request.Version)) return Results.BadRequest("version is required");
    await clientLock.WaitAsync();
    try
    {
        if (clientProcess is not null && !clientProcess.HasExited) return Results.Conflict("a client is already running");
        await EnsureDisplay();
        var dir = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft";
        clientProcess = LaunchPortablemc(dir, $"mojang:{request.Version}", request.Arguments ?? []);
        return Results.Ok(new { status = "started", pid = clientProcess?.Id });
    }
    finally { clientLock.Release(); }
});

application.MapPost("/start-curseforge", async (HttpContext context) =>
{
    var request = await context.Request.ReadFromJsonAsync<CurseForgeRequest>();
    if (request is null || string.IsNullOrWhiteSpace(request.Slug) || request.FileId <= 0)
        return Results.BadRequest("slug and positive fileId are required");
    var apiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey)) return Results.Problem("CURSEFORGE_API_KEY is not set");
    await clientLock.WaitAsync();
    try
    {
        if (clientProcess is not null && !clientProcess.HasExited) return Results.Conflict("a client is already running");
        var dir = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft";
        Directory.CreateDirectory(dir);
        var markerFile = Path.Combine(dir, ".curseforge-modpack");
        var versionFile = Path.Combine(dir, ".curseforge-portablemc-version");
        var marker = $"{request.Slug} {request.FileId}";
        var existing = File.Exists(markerFile) ? (await File.ReadAllTextAsync(markerFile)).Trim() : "";
        string version;
        if (existing == marker && File.Exists(versionFile))
            version = (await File.ReadAllTextAsync(versionFile)).Trim();
        else
        {
            version = await InstallModpack(request.Slug, request.FileId, apiKey, dir);
            await File.WriteAllTextAsync(versionFile, version);
            await File.WriteAllTextAsync(markerFile, marker);
        }
        await EnsureDisplay();
        clientProcess = LaunchPortablemc(dir, version, request.Arguments ?? []);
        return Results.Ok(new { status = "started", pid = clientProcess?.Id });
    }
    finally { clientLock.Release(); }
});

application.MapPost("/stop-client", async () =>
{
    await clientLock.WaitAsync();
    try
    {
        if (clientProcess is null || clientProcess.HasExited) { clientProcess = null; return Results.NotFound("no client is running"); }
        clientProcess.Kill(entireProcessTree: true);
        await clientProcess.WaitForExitAsync();
        clientProcess = null;
        return Results.Ok(new { status = "stopped" });
    }
    finally { clientLock.Release(); }
});

application.MapPost("/send-chat", async (HttpContext context) =>
{
    var request = await context.Request.ReadFromJsonAsync<ChatRequest>();
    if (request is null || string.IsNullOrWhiteSpace(request.Message)) return Results.BadRequest("message is required");
    if (clientProcess is null || clientProcess.HasExited) return Results.Conflict("no client is running");
    var windowId = await FindLargestWindow();
    if (windowId is null) return Results.Problem("no visible window found");
    await Run("xdotool", "windowfocus", windowId);
    await Run("xdotool", "key", "--window", windowId, "t");
    await Task.Delay(200);
    await Run("xdotool", "type", "--delay", "50", "--", request.Message);
    await Run("xdotool", "key", "Return");
    return Results.Ok(new { status = "sent" });
});

application.MapGet("/screen", async () =>
{
    if (clientProcess is null || clientProcess.HasExited) return Results.Conflict("no client is running");
    var windowId = await FindLargestWindow();
    if (windowId is null) return Results.Problem("no visible window found");
    var info = new ProcessStartInfo("import") { RedirectStandardOutput = true };
    info.ArgumentList.Add("-window"); info.ArgumentList.Add(windowId); info.ArgumentList.Add("png:-");
    using var process = Process.Start(info);
    if (process is null) return Results.Problem("failed to capture screen");
    using var stream = new MemoryStream();
    await process.StandardOutput.BaseStream.CopyToAsync(stream);
    await process.WaitForExitAsync();
    return Results.File(stream.ToArray(), "image/png");
});

application.Run();

Process? LaunchPortablemc(string directory, string version, string[] arguments)
{
    var info = new ProcessStartInfo("portablemc") { RedirectStandardOutput = true, RedirectStandardError = true };
    info.ArgumentList.Add("--main-dir"); info.ArgumentList.Add(directory);
    info.ArgumentList.Add("start"); info.ArgumentList.Add(version);
    foreach (var a in arguments) info.ArgumentList.Add(a);
    return Process.Start(info);
}

async Task EnsureDisplay()
{
    var display = Environment.GetEnvironmentVariable("DISPLAY");
    if (!string.IsNullOrEmpty(display))
    {
        using var check = Process.Start(new ProcessStartInfo("xdpyinfo") { RedirectStandardOutput = true, RedirectStandardError = true });
        if (check is not null) { await check.WaitForExitAsync(); if (check.ExitCode == 0) return; }
    }
    display = ":99";
    Environment.SetEnvironmentVariable("DISPLAY", display);
    if (File.Exists("/tmp/.X99-lock")) File.Delete("/tmp/.X99-lock");
    var xvfb = new ProcessStartInfo("Xvfb") { RedirectStandardOutput = true, RedirectStandardError = true };
    xvfb.ArgumentList.Add(display); xvfb.ArgumentList.Add("-screen"); xvfb.ArgumentList.Add("0"); xvfb.ArgumentList.Add("1280x720x24");
    Process.Start(xvfb);
    for (var i = 0; i < 50; i++)
    {
        await Task.Delay(100);
        using var r = Process.Start(new ProcessStartInfo("xdpyinfo") { RedirectStandardOutput = true, RedirectStandardError = true });
        if (r is not null) { await r.WaitForExitAsync(); if (r.ExitCode == 0) break; }
    }
    Process.Start(new ProcessStartInfo("xfwm4") { RedirectStandardOutput = true, RedirectStandardError = true });
}

async Task<string?> FindLargestWindow()
{
    var info = new ProcessStartInfo("xdotool") { RedirectStandardOutput = true, RedirectStandardError = true };
    info.ArgumentList.Add("search"); info.ArgumentList.Add("--onlyvisible"); info.ArgumentList.Add("--name"); info.ArgumentList.Add(".*");
    using var process = Process.Start(info);
    if (process is null) return null;
    var output = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();
    string? largest = null; long largestArea = 0;
    foreach (var candidate in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
    {
        var geo = new ProcessStartInfo("xdotool") { RedirectStandardOutput = true, RedirectStandardError = true };
        geo.ArgumentList.Add("getwindowgeometry"); geo.ArgumentList.Add("--shell"); geo.ArgumentList.Add(candidate.Trim());
        using var g = Process.Start(geo);
        if (g is null) continue;
        var gOut = await g.StandardOutput.ReadToEndAsync(); await g.WaitForExitAsync();
        if (g.ExitCode != 0) continue;
        int w = 0, h = 0;
        foreach (var line in gOut.Split('\n'))
        {
            if (line.StartsWith("WIDTH=")) int.TryParse(line["WIDTH=".Length..], out w);
            else if (line.StartsWith("HEIGHT=")) int.TryParse(line["HEIGHT=".Length..], out h);
        }
        if ((long)w * h > largestArea) { largestArea = (long)w * h; largest = candidate.Trim(); }
    }
    return largest;
}

async Task Run(params string[] command)
{
    var info = new ProcessStartInfo(command[0]) { RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var a in command[1..]) info.ArgumentList.Add(a);
    using var p = Process.Start(info);
    if (p is not null) await p.WaitForExitAsync();
}

async Task<string> InstallModpack(string slug, int fileId, string apiKey, string mcDir)
{
    var client = new ApiClient(apiKey);
    var searchResult = await client.SearchModsAsync(432, searchFilter: slug);
    var project = searchResult.Data.FirstOrDefault(m => m.Slug == slug)
        ?? throw new InvalidOperationException($"modpack not found: {slug}");
    var fileResponse = await client.GetModFileAsync(project.Id, fileId);
    var url = fileResponse.Data.DownloadUrl ?? $"https://www.curseforge.com/api/v1/mods/{project.Id}/files/{fileId}/download";
    using var httpClient = new HttpClient();
    var bytes = await httpClient.GetByteArrayAsync(url);
    using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
    var manifestEntry = archive.GetEntry("manifest.json") ?? throw new InvalidOperationException("manifest.json not found");
    using var ms = manifestEntry.Open();
    var manifest = await JsonDocument.ParseAsync(ms);
    var overrides = manifest.RootElement.TryGetProperty("overrides", out var op) ? op.GetString() ?? "overrides" : "overrides";
    foreach (var entry in archive.Entries)
    {
        if (!entry.FullName.StartsWith(overrides + "/") || entry.FullName.Length <= overrides.Length + 1) continue;
        var target = Path.Combine(mcDir, entry.FullName[(overrides.Length + 1)..]);
        if (entry.FullName.EndsWith('/')) { Directory.CreateDirectory(target); continue; }
        Directory.CreateDirectory(Path.GetDirectoryName(target) ?? mcDir);
        entry.ExtractToFile(target, overwrite: true);
    }
    var modsDir = Path.Combine(mcDir, "mods"); Directory.CreateDirectory(modsDir);
    if (manifest.RootElement.TryGetProperty("files", out var filesArray))
    {
        var ids = new List<int>();
        foreach (var f in filesArray.EnumerateArray())
        {
            if (f.TryGetProperty("required", out var req) && req.ValueKind == JsonValueKind.False) continue;
            if (f.TryGetProperty("fileID", out var fid)) ids.Add(fid.GetInt32());
            else if (f.TryGetProperty("fileId", out var fid2)) ids.Add(fid2.GetInt32());
        }
        if (ids.Count > 0)
        {
            var response = await client.GetFilesAsync(ids);
            foreach (var meta in response.Data)
            {
                var dl = meta.DownloadUrl ?? $"https://www.curseforge.com/api/v1/mods/{meta.ModId}/files/{meta.Id}/download";
                var dir = meta.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                    ? Path.Combine(mcDir, meta.FileName.Contains("shader", StringComparison.OrdinalIgnoreCase) ? "shaderpacks" : "resourcepacks")
                    : modsDir;
                Directory.CreateDirectory(dir);
                var dest = Path.Combine(dir, meta.FileName);
                if (!File.Exists(dest)) await File.WriteAllBytesAsync(dest, await httpClient.GetByteArrayAsync(dl));
            }
        }
    }
    var mcVersion = manifest.RootElement.GetProperty("minecraft").GetProperty("version").GetString()
        ?? throw new InvalidOperationException("minecraft.version missing");
    var pmc = $"mojang:{mcVersion}";
    if (manifest.RootElement.GetProperty("minecraft").TryGetProperty("modLoaders", out var loaders))
        foreach (var loader in loaders.EnumerateArray())
        {
            if (!loader.TryGetProperty("primary", out var primary) || !primary.GetBoolean()) continue;
            var id = loader.GetProperty("id").GetString() ?? "";
            if (id.StartsWith("neoforge-")) pmc = $"neoforge::{id["neoforge-".Length..]}";
            else if (id.StartsWith("forge-"))
            { var fv = id["forge-".Length..]; pmc = fv.StartsWith($"{mcVersion}-") ? $"forge::{fv}" : $"forge::{mcVersion}-{fv}"; }
            else if (id.StartsWith("fabric-")) pmc = $"fabric:{mcVersion}:{id["fabric-".Length..]}";
            else if (id.StartsWith("quilt-")) pmc = $"quilt:{mcVersion}:{id["quilt-".Length..]}";
            break;
        }
    return pmc;
}

record VanillaRequest(string? Version, string[]? Arguments);
record CurseForgeRequest(string? Slug, int FileId, string[]? Arguments);
record ChatRequest(string? Message);
