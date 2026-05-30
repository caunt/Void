#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net11.0
#:property PublishAot=false
#:package CurseForge.APIClient@*

using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using CurseForge.APIClient;
using GetModFilesRequestBody = CurseForge.APIClient.Models.Files.GetModFilesRequestBody;
using CurseForgeFile = CurseForge.APIClient.Models.Files.File;

const string DefaultMinecraftDirectory = "/root/.minecraft";
const string DefaultDisplay = ":99";
const string DisplayScreen = "1280x720x24";
const int MinecraftGameId = 432;
const int CurseForgeFilesBatchSize = 50;
const int BrightnessThreshold = 5;
const int MaximumChatOpenPresses = 100;
const int DisplayReadyMaxAttempts = 50;
const int DisplayReadyDelayMilliseconds = 100;

var builder = WebApplication.CreateBuilder(args);
var application = builder.Build();
var clientProcess = (Process?)null;
var clientLock = new SemaphoreSlim(1, 1);

application.MapGet("/health", () => "ok");

application.MapGet("/start-vanilla", async (HttpContext httpContext, string? version) =>
{
    if (string.IsNullOrWhiteSpace(version))
    {
        return Results.BadRequest("version is required");
    }

    var portableMinecraftArguments = httpContext.Request.Query["argument"].ToArray();

    await clientLock.WaitAsync();

    try
    {
        if (clientProcess is not null && !clientProcess.HasExited)
        {
            return Results.Conflict("a client is already running");
        }

        await EnsureDisplay();
        var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? DefaultMinecraftDirectory;
        var portablemcVersion = $"mojang:{version}";
        Console.Error.WriteLine($"Launching Minecraft with PortableMC version: {portablemcVersion}");
        clientProcess = LaunchPortablemc(minecraftDirectory, portablemcVersion, portableMinecraftArguments);
        return Results.Ok(new { status = "started", pid = clientProcess?.Id });
    }
    finally
    {
        clientLock.Release();
    }
});

application.MapGet("/start-curseforge", async (HttpContext httpContext, string? slug, int fileId) =>
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

    var portableMinecraftArguments = httpContext.Request.Query["argument"].ToArray();

    await clientLock.WaitAsync();

    try
    {
        if (clientProcess is not null && !clientProcess.HasExited)
        {
            return Results.Conflict("a client is already running");
        }

        var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? DefaultMinecraftDirectory;
        Directory.CreateDirectory(minecraftDirectory);
        var markerFile = Path.Combine(minecraftDirectory, ".curseforge-modpack");
        var versionFile = Path.Combine(minecraftDirectory, ".curseforge-portablemc-version");
        var marker = $"{slug} {fileId}";
        var existingMarker = File.Exists(markerFile) ? (await File.ReadAllTextAsync(markerFile)).Trim() : "";
        string portablemcVersion;

        Console.Error.WriteLine($"Starting CurseForge modpack '{slug}' file '{fileId}'");

        if (existingMarker == marker)
        {
            if (!File.Exists(versionFile))
            {
                return Results.Problem($"PortableMC version cache file is missing: {versionFile}");
            }

            portablemcVersion = (await File.ReadAllTextAsync(versionFile)).Trim();
            Console.Error.WriteLine($"Using existing installation in {minecraftDirectory}");
        }
        else
        {
            portablemcVersion = await InstallModpack(slug, fileId, curseForgeApiKey, minecraftDirectory);
            await File.WriteAllTextAsync(versionFile, portablemcVersion);
            await File.WriteAllTextAsync(markerFile, marker);
            Console.Error.WriteLine("Installation marker updated");
        }

        await EnsureDisplay();
        Console.Error.WriteLine($"Launching Minecraft with PortableMC version: {portablemcVersion}");
        clientProcess = LaunchPortablemc(minecraftDirectory, portablemcVersion, portableMinecraftArguments);
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

    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
    var windowId = await FindLargestWindow(display);

    if (windowId is null)
    {
        return Results.Problem("no visible window found");
    }

    await RunOrThrow("xdotool", "windowfocus", windowId);
    var chatOpened = await OpenChatAsync(windowId, display);

    if (!chatOpened)
    {
        return Results.Problem("failed to open chat interface after maximum attempts");
    }

    await RunOrThrow("xdotool", "type", "--clearmodifiers", "--window", windowId, "--delay", "50", "--", message);
    await RunOrThrow("xdotool", "key", "--clearmodifiers", "--window", windowId, "Return");
    return Results.Ok(new { status = "sent" });
});

application.MapGet("/screen", async () =>
{
    if (clientProcess is null || clientProcess.HasExited)
    {
        return Results.Conflict("no client is running");
    }

    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
    var windowId = await FindLargestWindow(display);

    if (windowId is null)
    {
        return Results.Problem("no visible window found");
    }

    var captureProcessInfo = new ProcessStartInfo("import")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    captureProcessInfo.Environment["DISPLAY"] = display;
    captureProcessInfo.ArgumentList.Add("-window");
    captureProcessInfo.ArgumentList.Add(windowId);
    captureProcessInfo.ArgumentList.Add("png:-");

    using var captureProcess = Process.Start(captureProcessInfo);

    if (captureProcess is null)
    {
        return Results.Problem("failed to capture screen");
    }

    using var imageStream = new MemoryStream();
    var standardError = captureProcess.StandardError.ReadToEndAsync();
    await captureProcess.StandardOutput.BaseStream.CopyToAsync(imageStream);
    await captureProcess.WaitForExitAsync();
    var errorOutput = await standardError;

    if (captureProcess.ExitCode != 0)
    {
        return Results.Problem($"screen capture failed: {errorOutput}");
    }

    return Results.File(imageStream.ToArray(), "image/png");
});

application.Run();

Process? LaunchPortablemc(string directory, string version, string[] portableMinecraftArguments)
{
    var processInfo = new ProcessStartInfo("portablemc");
    processInfo.ArgumentList.Add("--main-dir");
    processInfo.ArgumentList.Add(directory);
    processInfo.ArgumentList.Add("start");
    processInfo.ArgumentList.Add(version);

    foreach (var argument in portableMinecraftArguments)
    {
        processInfo.ArgumentList.Add(argument);
    }

    return Process.Start(processInfo);
}

async Task EnsureDisplay()
{
    var display = Environment.GetEnvironmentVariable("DISPLAY");

    if (!string.IsNullOrEmpty(display))
    {
        var checkProcessInfo = new ProcessStartInfo("xdpyinfo")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        checkProcessInfo.Environment["DISPLAY"] = display;
        checkProcessInfo.ArgumentList.Add("-display");
        checkProcessInfo.ArgumentList.Add(display);

        using var checkProcess = Process.Start(checkProcessInfo);

        if (checkProcess is not null)
        {
            await checkProcess.WaitForExitAsync();

            if (checkProcess.ExitCode == 0)
            {
                return;
            }
        }
    }

    display ??= DefaultDisplay;
    Environment.SetEnvironmentVariable("DISPLAY", display);

    var displayNumber = display.TrimStart(':');
    var lockFile = $"/tmp/.X{displayNumber}-lock";

    if (File.Exists(lockFile))
    {
        File.Delete(lockFile);
    }

    var xvfbProcessInfo = new ProcessStartInfo("Xvfb");
    xvfbProcessInfo.ArgumentList.Add(display);
    xvfbProcessInfo.ArgumentList.Add("-screen");
    xvfbProcessInfo.ArgumentList.Add("0");
    xvfbProcessInfo.ArgumentList.Add(DisplayScreen);
    Process.Start(xvfbProcessInfo);

    var displayIsReady = false;

    for (var attempt = 0; attempt < DisplayReadyMaxAttempts; attempt++)
    {
        await Task.Delay(DisplayReadyDelayMilliseconds);

        var readyCheckInfo = new ProcessStartInfo("xdpyinfo")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        readyCheckInfo.Environment["DISPLAY"] = display;
        readyCheckInfo.ArgumentList.Add("-display");
        readyCheckInfo.ArgumentList.Add(display);

        using var readyCheck = Process.Start(readyCheckInfo);

        if (readyCheck is not null)
        {
            await readyCheck.WaitForExitAsync();

            if (readyCheck.ExitCode == 0)
            {
                displayIsReady = true;
                break;
            }
        }
    }

    if (!displayIsReady)
    {
        throw new InvalidOperationException($"display {display} did not become ready after {DisplayReadyMaxAttempts} attempts");
    }

    var xfwm4ProcessInfo = new ProcessStartInfo("xfwm4");
    xfwm4ProcessInfo.Environment["DISPLAY"] = display;
    Process.Start(xfwm4ProcessInfo);
}

async Task<bool> OpenChatAsync(string windowId, string display)
{
    var baselineBrightness = await CaptureBrightnessAsync(windowId, display);
    var currentPressCount = 0;

    while (currentPressCount < MaximumChatOpenPresses)
    {
        await RunOrThrow("xdotool", "key", "--clearmodifiers", "--window", windowId, "t");
        currentPressCount++;

        await Task.Delay(132);

        var currentBrightness = await CaptureBrightnessAsync(windowId, display);
        var brightnessDifference = Math.Abs(baselineBrightness - currentBrightness);

        if (brightnessDifference > BrightnessThreshold)
        {
            for (var backspaceIndex = 0; backspaceIndex < currentPressCount; backspaceIndex++)
            {
                await RunOrThrow("xdotool", "key", "--clearmodifiers", "--window", windowId, "--delay", "50", "BackSpace");
            }

            return true;
        }
    }

    return false;
}

async Task<int> CaptureBrightnessAsync(string windowId, string display)
{
    var importProcessInfo = new ProcessStartInfo("import")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    importProcessInfo.Environment["DISPLAY"] = display;
    importProcessInfo.ArgumentList.Add("-window");
    importProcessInfo.ArgumentList.Add(windowId);
    importProcessInfo.ArgumentList.Add("miff:-");

    using var importProcess = Process.Start(importProcessInfo)
        ?? throw new InvalidOperationException("failed to start import for brightness capture");

    var convertProcessInfo = new ProcessStartInfo("convert")
    {
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    convertProcessInfo.ArgumentList.Add("-");
    convertProcessInfo.ArgumentList.Add("-gravity");
    convertProcessInfo.ArgumentList.Add("SouthWest");
    convertProcessInfo.ArgumentList.Add("-crop");
    convertProcessInfo.ArgumentList.Add("100x20+0+0");
    convertProcessInfo.ArgumentList.Add("+repage");
    convertProcessInfo.ArgumentList.Add("-format");
    convertProcessInfo.ArgumentList.Add("%[fx:int(mean*100)]");
    convertProcessInfo.ArgumentList.Add("info:");

    using var convertProcess = Process.Start(convertProcessInfo)
        ?? throw new InvalidOperationException("failed to start convert for brightness capture");

    var importStandardError = importProcess.StandardError.ReadToEndAsync();
    await importProcess.StandardOutput.BaseStream.CopyToAsync(convertProcess.StandardInput.BaseStream);
    convertProcess.StandardInput.Close();

    var brightnessOutput = await convertProcess.StandardOutput.ReadToEndAsync();
    var convertStandardError = await convertProcess.StandardError.ReadToEndAsync();
    var importErrorOutput = await importStandardError;
    await importProcess.WaitForExitAsync();
    await convertProcess.WaitForExitAsync();

    if (importProcess.ExitCode != 0)
    {
        throw new InvalidOperationException(
            $"import exited with code {importProcess.ExitCode} during brightness capture: {importErrorOutput}");
    }

    if (convertProcess.ExitCode != 0)
    {
        throw new InvalidOperationException(
            $"convert exited with code {convertProcess.ExitCode} during brightness capture: {convertStandardError}");
    }

    if (!int.TryParse(brightnessOutput.Trim(), out var brightness))
    {
        throw new InvalidOperationException(
            $"failed to parse brightness value from convert output: '{brightnessOutput.Trim()}'");
    }

    return brightness;
}

async Task<string?> FindLargestWindow(string display)
{
    var searchProcessInfo = new ProcessStartInfo("xdotool")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    searchProcessInfo.Environment["DISPLAY"] = display;
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
        var trimmedCandidateId = candidateWindowId.Trim();

        var nameProcessInfo = new ProcessStartInfo("xdotool")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        nameProcessInfo.Environment["DISPLAY"] = display;
        nameProcessInfo.ArgumentList.Add("getwindowname");
        nameProcessInfo.ArgumentList.Add(trimmedCandidateId);

        using var nameProcess = Process.Start(nameProcessInfo);

        if (nameProcess is null)
        {
            continue;
        }

        var windowName = await nameProcess.StandardOutput.ReadToEndAsync();
        await nameProcess.WaitForExitAsync();

        if (string.IsNullOrWhiteSpace(windowName))
        {
            continue;
        }

        var geometryProcessInfo = new ProcessStartInfo("xdotool")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        geometryProcessInfo.Environment["DISPLAY"] = display;
        geometryProcessInfo.ArgumentList.Add("getwindowgeometry");
        geometryProcessInfo.ArgumentList.Add("--shell");
        geometryProcessInfo.ArgumentList.Add(trimmedCandidateId);

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
            largestWindowId = trimmedCandidateId;
        }
    }

    return largestWindowId;
}

async Task RunOrThrow(params string[] command)
{
    var processInfo = new ProcessStartInfo(command[0])
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };

    foreach (var argument in command[1..])
    {
        processInfo.ArgumentList.Add(argument);
    }

    using var process = Process.Start(processInfo);

    if (process is null)
    {
        throw new InvalidOperationException($"failed to start {command[0]}");
    }

    var standardError = await process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();

    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException($"{command[0]} exited with code {process.ExitCode}: {standardError}");
    }
}

void DeleteDirectoryIfExists(string path)
{
    if (Directory.Exists(path))
    {
        Directory.Delete(path, recursive: true);
    }
}

void ValidateFileName(string fileName, int modId, int modFileId)
{
    var safeName = Path.GetFileName(fileName);

    if (safeName != fileName
        || string.IsNullOrEmpty(safeName)
        || safeName == "."
        || safeName == ".."
        || safeName.Contains('/')
        || safeName.Contains('\\')
        || safeName.Any(character => char.IsControl(character)))
    {
        throw new InvalidOperationException(
            $"Unexpected CurseForge file name '{fileName}' for mod id '{modId}' and file id '{modFileId}'");
    }
}

string DetermineTargetDirectory(string fileName, string minecraftDirectory)
{
    if (fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase))
    {
        return Path.Combine(minecraftDirectory, "mods");
    }

    if (fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
    {
        if (fileName.Contains("shader", StringComparison.OrdinalIgnoreCase))
        {
            return Path.Combine(minecraftDirectory, "shaderpacks");
        }

        return Path.Combine(minecraftDirectory, "resourcepacks");
    }

    return Path.Combine(minecraftDirectory, "mods");
}

async Task<string> InstallModpack(string slug, int fileId, string apiKey, string minecraftDirectory)
{
    var curseForgeClient = new ApiClient(apiKey);
    using var httpClient = new HttpClient();

    Console.Error.WriteLine("Resolving CurseForge project");
    var searchResult = await curseForgeClient.SearchModsAsync(gameId: MinecraftGameId, slug: slug);
    var project = searchResult.Data.FirstOrDefault(modpack => modpack.Slug == slug)
        ?? throw new InvalidOperationException($"modpack not found: {slug}");

    Console.Error.WriteLine("Downloading modpack archive");
    var archiveDownloadUrl = await ResolveDownloadUrlAsync(curseForgeClient, httpClient, project.Id, fileId, apiKey);
    var archiveBytes = await DownloadWithFallbackAsync(httpClient, archiveDownloadUrl, apiKey);

    Console.Error.WriteLine("Reading modpack manifest");
    using var archive = new ZipArchive(new MemoryStream(archiveBytes), ZipArchiveMode.Read);
    var manifestEntry = archive.GetEntry("manifest.json")
        ?? throw new InvalidOperationException("manifest.json not found");

    using var manifestStream = manifestEntry.Open();
    var manifest = await JsonSerializer.DeserializeAsync<CurseForgeManifest>(manifestStream, JsonSerializerOptions.Web)
        ?? throw new InvalidOperationException("failed to deserialize manifest.json");

    DeleteDirectoryIfExists(Path.Combine(minecraftDirectory, "mods"));
    DeleteDirectoryIfExists(Path.Combine(minecraftDirectory, "resourcepacks"));
    DeleteDirectoryIfExists(Path.Combine(minecraftDirectory, "shaderpacks"));
    Directory.CreateDirectory(Path.Combine(minecraftDirectory, "mods"));
    Directory.CreateDirectory(Path.Combine(minecraftDirectory, "resourcepacks"));
    Directory.CreateDirectory(Path.Combine(minecraftDirectory, "shaderpacks"));
    Console.Error.WriteLine($"Prepared Minecraft directory: {minecraftDirectory}");

    var overridesFolder = manifest.Overrides ?? "overrides";

    if (archive.Entries.Any(entry => entry.FullName.StartsWith(overridesFolder + "/")))
    {
        Console.Error.WriteLine("Installing modpack overrides");

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
    }

    if (manifest.Files is { Count: > 0 })
    {
        var requiredFileIds = new List<int>();

        foreach (var file in manifest.Files)
        {
            if (file.Required == false)
            {
                continue;
            }

            var resolvedFileId = file.FileID ?? file.FileId;

            if (resolvedFileId is > 0)
            {
                requiredFileIds.Add(resolvedFileId.Value);
            }
        }

        if (requiredFileIds.Count > 0)
        {
            Console.Error.WriteLine($"Resolving {requiredFileIds.Count} CurseForge files");

            var allFileMetadata = new List<CurseForgeFile>();

            for (var batchStart = 0; batchStart < requiredFileIds.Count; batchStart += CurseForgeFilesBatchSize)
            {
                var batch = requiredFileIds.Skip(batchStart).Take(CurseForgeFilesBatchSize).ToList();
                var filesResponse = await curseForgeClient.GetFilesAsync(new GetModFilesRequestBody { FileIds = batch });
                allFileMetadata.AddRange(filesResponse.Data);
            }

            var totalFileCount = allFileMetadata.Count;
            var downloadIndex = 0;

            foreach (var fileMeta in allFileMetadata)
            {
                downloadIndex++;
                ValidateFileName(fileMeta.FileName, fileMeta.ModId, fileMeta.Id);

                var targetDirectory = DetermineTargetDirectory(fileMeta.FileName, minecraftDirectory);
                Directory.CreateDirectory(targetDirectory);
                var destinationPath = Path.Combine(targetDirectory, fileMeta.FileName);

                if (File.Exists(destinationPath))
                {
                    Console.Error.WriteLine($"[{downloadIndex}/{totalFileCount}] Already exists: {fileMeta.FileName}");
                    continue;
                }

                Console.Error.WriteLine($"[{downloadIndex}/{totalFileCount}] Downloading: {fileMeta.FileName}");

                var fileDownloadUrl = await ResolveModFileDownloadUrlAsync(
                    curseForgeClient, httpClient, fileMeta.ModId, fileMeta.Id, fileMeta.DownloadUrl, apiKey);
                var fileBytes = await DownloadWithFallbackAsync(httpClient, fileDownloadUrl, apiKey);
                await File.WriteAllBytesAsync(destinationPath, fileBytes);
            }
        }
        else
        {
            Console.Error.WriteLine("No CurseForge files to download");
        }
    }
    else
    {
        Console.Error.WriteLine("No CurseForge files to download");
    }

    Console.Error.WriteLine("Resolving PortableMC version");
    var minecraftVersion = manifest.Minecraft?.Version
        ?? throw new InvalidOperationException("minecraft.version missing");
    var portablemcVersion = $"mojang:{minecraftVersion}";

    if (manifest.Minecraft.ModLoaders is not null)
    {
        foreach (var loader in manifest.Minecraft.ModLoaders)
        {
            if (loader.Primary != true)
            {
                continue;
            }

            var loaderId = loader.Id ?? "";

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
            else
            {
                Environment.FailFast(
                    $"Unsupported mod loader for CurseForge modpack '{slug}' (file id: {fileId}): {loaderId}");
            }

            break;
        }
    }

    return portablemcVersion;
}

async Task<string> ResolveDownloadUrlAsync(ApiClient curseForgeClient, HttpClient httpClient, int projectId, int modFileId, string apiKey)
{
    var fileResponse = await curseForgeClient.GetModFileAsync(projectId, modFileId);

    if (!string.IsNullOrEmpty(fileResponse.Data.DownloadUrl))
    {
        return fileResponse.Data.DownloadUrl;
    }

    var urlResponse = await curseForgeClient.GetModFileDownloadUrlAsync(projectId, modFileId);

    if (!string.IsNullOrEmpty(urlResponse.Data))
    {
        return urlResponse.Data;
    }

    return $"https://www.curseforge.com/api/v1/mods/{projectId}/files/{modFileId}/download";
}

async Task<string> ResolveModFileDownloadUrlAsync(ApiClient curseForgeClient, HttpClient httpClient, int modId, int modFileId, string? sdkDownloadUrl, string apiKey)
{
    if (!string.IsNullOrEmpty(sdkDownloadUrl))
    {
        return sdkDownloadUrl;
    }

    var urlResponse = await curseForgeClient.GetModFileDownloadUrlAsync(modId, modFileId);

    if (!string.IsNullOrEmpty(urlResponse.Data))
    {
        return urlResponse.Data;
    }

    return $"https://www.curseforge.com/api/v1/mods/{modId}/files/{modFileId}/download";
}

async Task<byte[]> DownloadWithFallbackAsync(HttpClient httpClient, string downloadUrl, string apiKey)
{
    var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);

    if (downloadUrl.Contains("curseforge.com", StringComparison.OrdinalIgnoreCase))
    {
        request.Headers.TryAddWithoutValidation("x-api-key", apiKey);
    }

    var response = await httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsByteArrayAsync();
}

record CurseForgeManifest(
    CurseForgeMinecraft? Minecraft,
    string? Overrides,
    List<CurseForgeManifestFile>? Files);

record CurseForgeMinecraft(
    string? Version,
    List<CurseForgeModLoader>? ModLoaders);

record CurseForgeModLoader(
    string? Id,
    bool? Primary);

record CurseForgeManifestFile(
    [property: JsonPropertyName("fileID")] int? FileID,
    [property: JsonPropertyName("fileId")] int? FileId,
    bool? Required);
