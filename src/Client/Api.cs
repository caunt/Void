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

ProcessStartInfo CreateProcessStartInfo(string fileName)
{
    return new ProcessStartInfo(fileName)
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
}

application.MapGet("/health", () => "ok");

application.MapGet("/start-vanilla", async (string? version) =>
{
    if (string.IsNullOrWhiteSpace(version))
    {
        return Results.BadRequest("version is required");
    }

    await clientLock.WaitAsync();

    try
    {
        if (clientProcess is not null && !clientProcess.HasExited)
        {
            return Results.Conflict("a client is already running");
        }

        await EnsureDisplay();
        var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft";
        clientProcess = LaunchPortablemc(minecraftDirectory, $"mojang:{version}");
        return Results.Ok(new { status = "started", pid = clientProcess?.Id });
    }
    finally
    {
        clientLock.Release();
    }
});

application.MapGet("/start-curseforge", async (string? slug, int fileId) =>
{
    if (string.IsNullOrWhiteSpace(slug) || fileId <= 0)
    {
        return Results.BadRequest("slug and positive fileId are required");
    }

    var curseForgeApiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");

    if (string.IsNullOrWhiteSpace(curseForgeApiKey))
    {
        return Results.Problem("CURSEFORGE_API_KEY is not set");
    }

    await clientLock.WaitAsync();

    try
    {
        if (clientProcess is not null && !clientProcess.HasExited)
        {
            return Results.Conflict("a client is already running");
        }

        var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft";
        Directory.CreateDirectory(minecraftDirectory);
        var markerFile = Path.Combine(minecraftDirectory, ".curseforge-modpack");
        var versionFile = Path.Combine(minecraftDirectory, ".curseforge-portablemc-version");
        var marker = $"{slug} {fileId}";
        var existingMarker = File.Exists(markerFile) ? (await File.ReadAllTextAsync(markerFile)).Trim() : "";
        string portablemcVersion;

        if (existingMarker == marker && File.Exists(versionFile))
        {
            portablemcVersion = (await File.ReadAllTextAsync(versionFile)).Trim();
        }
        else
        {
            portablemcVersion = await InstallModpack(slug, fileId, curseForgeApiKey, minecraftDirectory);
            await File.WriteAllTextAsync(versionFile, portablemcVersion);
            await File.WriteAllTextAsync(markerFile, marker);
        }

        await EnsureDisplay();
        clientProcess = LaunchPortablemc(minecraftDirectory, portablemcVersion);
        return Results.Ok(new { status = "started", pid = clientProcess?.Id });
    }
    finally
    {
        clientLock.Release();
    }
});

application.MapGet("/stop-client", async () =>
{
    await clientLock.WaitAsync();

    try
    {
        if (clientProcess is null || clientProcess.HasExited)
        {
            clientProcess = null;
            return Results.NotFound("no client is running");
        }

        clientProcess.Kill(entireProcessTree: true);
        await clientProcess.WaitForExitAsync();
        clientProcess = null;
        return Results.Ok(new { status = "stopped" });
    }
    finally
    {
        clientLock.Release();
    }
});

application.MapGet("/send-chat", async (string? message) =>
{
    if (string.IsNullOrWhiteSpace(message))
    {
        return Results.BadRequest("message is required");
    }

    if (clientProcess is null || clientProcess.HasExited)
    {
        return Results.Conflict("no client is running");
    }

    var windowId = await FindLargestWindow();

    if (windowId is null)
    {
        return Results.Problem("no visible window found");
    }

    await Run("xdotool", "windowfocus", windowId);
    await Run("xdotool", "key", "--clearmodifiers", "--window", windowId, "t");
    await Task.Delay(200);
    await Run("xdotool", "type", "--clearmodifiers", "--window", windowId, "--delay", "50", "--", message);
    await Run("xdotool", "key", "--clearmodifiers", "--window", windowId, "Return");
    return Results.Ok(new { status = "sent" });
});

application.MapGet("/screen", async () =>
{
    if (clientProcess is null || clientProcess.HasExited)
    {
        return Results.Conflict("no client is running");
    }

    var windowId = await FindLargestWindow();

    if (windowId is null)
    {
        return Results.Problem("no visible window found");
    }

    var captureProcessInfo = CreateProcessStartInfo("import");
    captureProcessInfo.ArgumentList.Add("-window");
    captureProcessInfo.ArgumentList.Add(windowId);
    captureProcessInfo.ArgumentList.Add("png:-");

    using var captureProcess = Process.Start(captureProcessInfo);

    if (captureProcess is null)
    {
        return Results.Problem("failed to capture screen");
    }

    using var imageStream = new MemoryStream();
    await captureProcess.StandardOutput.BaseStream.CopyToAsync(imageStream);
    await captureProcess.WaitForExitAsync();
    return Results.File(imageStream.ToArray(), "image/png");
});

application.Run();

Process? LaunchPortablemc(string directory, string version)
{
    var processInfo = CreateProcessStartInfo("portablemc");
    processInfo.ArgumentList.Add("--main-dir");
    processInfo.ArgumentList.Add(directory);
    processInfo.ArgumentList.Add("start");
    processInfo.ArgumentList.Add(version);
    return Process.Start(processInfo);
}

async Task EnsureDisplay()
{
    var display = Environment.GetEnvironmentVariable("DISPLAY");

    if (!string.IsNullOrEmpty(display))
    {
        using var checkProcess = Process.Start(CreateProcessStartInfo("xdpyinfo"));

        if (checkProcess is not null)
        {
            await checkProcess.WaitForExitAsync();

            if (checkProcess.ExitCode == 0)
            {
                return;
            }
        }
    }

    display = ":99";
    Environment.SetEnvironmentVariable("DISPLAY", display);

    if (File.Exists("/tmp/.X99-lock"))
    {
        File.Delete("/tmp/.X99-lock");
    }

    var xvfbProcessInfo = CreateProcessStartInfo("Xvfb");
    xvfbProcessInfo.ArgumentList.Add(display);
    xvfbProcessInfo.ArgumentList.Add("-screen");
    xvfbProcessInfo.ArgumentList.Add("0");
    xvfbProcessInfo.ArgumentList.Add("1280x720x24");
    Process.Start(xvfbProcessInfo);

    for (var attempt = 0; attempt < 50; attempt++)
    {
        await Task.Delay(100);
        using var readyCheck = Process.Start(CreateProcessStartInfo("xdpyinfo"));

        if (readyCheck is not null)
        {
            await readyCheck.WaitForExitAsync();

            if (readyCheck.ExitCode == 0)
            {
                break;
            }
        }
    }

    Process.Start(CreateProcessStartInfo("xfwm4"));
}

async Task<string?> FindLargestWindow()
{
    var searchProcessInfo = CreateProcessStartInfo("xdotool");
    searchProcessInfo.ArgumentList.Add("search");
    searchProcessInfo.ArgumentList.Add("--onlyvisible");
    searchProcessInfo.ArgumentList.Add("--name");
    searchProcessInfo.ArgumentList.Add(".*");

    using var searchProcess = Process.Start(searchProcessInfo);

    if (searchProcess is null)
    {
        return null;
    }

    var searchOutput = await searchProcess.StandardOutput.ReadToEndAsync();
    await searchProcess.WaitForExitAsync();
    string? largestWindowId = null;
    long largestArea = 0;

    foreach (var candidateWindowId in searchOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
    {
        var geometryProcessInfo = CreateProcessStartInfo("xdotool");
        geometryProcessInfo.ArgumentList.Add("getwindowgeometry");
        geometryProcessInfo.ArgumentList.Add("--shell");
        geometryProcessInfo.ArgumentList.Add(candidateWindowId.Trim());

        using var geometryProcess = Process.Start(geometryProcessInfo);

        if (geometryProcess is null)
        {
            continue;
        }

        var geometryOutput = await geometryProcess.StandardOutput.ReadToEndAsync();
        await geometryProcess.WaitForExitAsync();

        if (geometryProcess.ExitCode != 0)
        {
            continue;
        }

        int width = 0, height = 0;

        foreach (var line in geometryOutput.Split('\n'))
        {
            if (line.StartsWith("WIDTH="))
            {
                int.TryParse(line["WIDTH=".Length..], out width);
            }
            else if (line.StartsWith("HEIGHT="))
            {
                int.TryParse(line["HEIGHT=".Length..], out height);
            }
        }

        if ((long)width * height > largestArea)
        {
            largestArea = (long)width * height;
            largestWindowId = candidateWindowId.Trim();
        }
    }

    return largestWindowId;
}

async Task Run(params string[] command)
{
    var processInfo = CreateProcessStartInfo(command[0]);

    foreach (var argument in command[1..])
    {
        processInfo.ArgumentList.Add(argument);
    }

    using var process = Process.Start(processInfo);

    if (process is not null)
    {
        await process.WaitForExitAsync();
    }
}

async Task<string> InstallModpack(string slug, int fileId, string apiKey, string minecraftDirectory)
{
    var curseForgeClient = new ApiClient(apiKey);
    var searchResult = await curseForgeClient.SearchModsAsync(432, searchFilter: slug);
    var project = searchResult.Data.FirstOrDefault(modpack => modpack.Slug == slug)
        ?? throw new InvalidOperationException($"modpack not found: {slug}");

    var fileResponse = await curseForgeClient.GetModFileAsync(project.Id, fileId);
    var downloadUrl = fileResponse.Data.DownloadUrl
        ?? $"https://www.curseforge.com/api/v1/mods/{project.Id}/files/{fileId}/download";

    using var httpClient = new HttpClient();
    var archiveBytes = await httpClient.GetByteArrayAsync(downloadUrl);
    using var archive = new ZipArchive(new MemoryStream(archiveBytes), ZipArchiveMode.Read);
    var manifestEntry = archive.GetEntry("manifest.json")
        ?? throw new InvalidOperationException("manifest.json not found");

    using var manifestStream = manifestEntry.Open();
    var manifest = await JsonDocument.ParseAsync(manifestStream);
    var overridesFolder = manifest.RootElement.TryGetProperty("overrides", out var overridesProperty)
        ? overridesProperty.GetString() ?? "overrides"
        : "overrides";

    foreach (var entry in archive.Entries)
    {
        if (!entry.FullName.StartsWith(overridesFolder + "/") || entry.FullName.Length <= overridesFolder.Length + 1)
        {
            continue;
        }

        var targetPath = Path.Combine(minecraftDirectory, entry.FullName[(overridesFolder.Length + 1)..]);

        if (entry.FullName.EndsWith('/'))
        {
            Directory.CreateDirectory(targetPath);
            continue;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? minecraftDirectory);
        entry.ExtractToFile(targetPath, overwrite: true);
    }

    var modsDirectory = Path.Combine(minecraftDirectory, "mods");
    Directory.CreateDirectory(modsDirectory);

    if (manifest.RootElement.TryGetProperty("files", out var filesArray))
    {
        var modFileIds = new List<int>();

        foreach (var file in filesArray.EnumerateArray())
        {
            if (file.TryGetProperty("required", out var requiredProperty) && requiredProperty.ValueKind == JsonValueKind.False)
            {
                continue;
            }

            if (file.TryGetProperty("fileID", out var fileIdProperty))
            {
                modFileIds.Add(fileIdProperty.GetInt32());
            }
            else if (file.TryGetProperty("fileId", out var fileIdAlternateProperty))
            {
                modFileIds.Add(fileIdAlternateProperty.GetInt32());
            }
        }

        if (modFileIds.Count > 0)
        {
            var filesResponse = await curseForgeClient.GetFilesAsync(modFileIds);

            foreach (var fileMeta in filesResponse.Data)
            {
                var fileDownloadUrl = fileMeta.DownloadUrl
                    ?? $"https://www.curseforge.com/api/v1/mods/{fileMeta.ModId}/files/{fileMeta.Id}/download";

                var destinationDirectory = fileMeta.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                    ? Path.Combine(minecraftDirectory, fileMeta.FileName.Contains("shader", StringComparison.OrdinalIgnoreCase) ? "shaderpacks" : "resourcepacks")
                    : modsDirectory;

                Directory.CreateDirectory(destinationDirectory);
                var destinationPath = Path.Combine(destinationDirectory, fileMeta.FileName);

                if (!File.Exists(destinationPath))
                {
                    await File.WriteAllBytesAsync(destinationPath, await httpClient.GetByteArrayAsync(fileDownloadUrl));
                }
            }
        }
    }

    var minecraftVersion = manifest.RootElement.GetProperty("minecraft").GetProperty("version").GetString()
        ?? throw new InvalidOperationException("minecraft.version missing");
    var portablemcVersion = $"mojang:{minecraftVersion}";

    if (manifest.RootElement.GetProperty("minecraft").TryGetProperty("modLoaders", out var loaders))
    {
        foreach (var loader in loaders.EnumerateArray())
        {
            if (!loader.TryGetProperty("primary", out var primary) || !primary.GetBoolean())
            {
                continue;
            }

            var loaderId = loader.GetProperty("id").GetString() ?? "";

            if (loaderId.StartsWith("neoforge-"))
            {
                portablemcVersion = $"neoforge::{loaderId["neoforge-".Length..]}";
            }
            else if (loaderId.StartsWith("forge-"))
            {
                var forgeVersion = loaderId["forge-".Length..];
                portablemcVersion = forgeVersion.StartsWith($"{minecraftVersion}-")
                    ? $"forge::{forgeVersion}"
                    : $"forge::{minecraftVersion}-{forgeVersion}";
            }
            else if (loaderId.StartsWith("fabric-"))
            {
                portablemcVersion = $"fabric:{minecraftVersion}:{loaderId["fabric-".Length..]}";
            }
            else if (loaderId.StartsWith("quilt-"))
            {
                portablemcVersion = $"quilt:{minecraftVersion}:{loaderId["quilt-".Length..]}";
            }

            break;
        }
    }

    return portablemcVersion;
}
