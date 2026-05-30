#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net11.0
#:property PublishAot=false

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ClientProcessManager>();
var application = builder.Build();

application.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

application.MapPost("/start-vanilla", async (HttpContext context, ClientProcessManager manager, ILogger<ClientProcessManager> logger) =>
{
    var request = await context.Request.ReadFromJsonAsync<StartVanillaRequest>();

    if (request is null || string.IsNullOrWhiteSpace(request.Version))
        return Results.BadRequest(new { error = "version is required" });

    var portableMinecraftVersion = $"mojang:{request.Version}";
    var result = await manager.StartAsync(portableMinecraftVersion, request.Arguments ?? [], logger);

    return result;
});

application.MapPost("/start-curseforge", async (HttpContext context, ClientProcessManager manager, ILogger<ClientProcessManager> logger) =>
{
    var request = await context.Request.ReadFromJsonAsync<StartCurseForgeRequest>();

    if (request is null || string.IsNullOrWhiteSpace(request.Slug))
        return Results.BadRequest(new { error = "slug is required" });

    if (request.FileId <= 0)
        return Results.BadRequest(new { error = "fileId must be a positive integer" });

    var curseForgeApiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");

    if (string.IsNullOrWhiteSpace(curseForgeApiKey))
        return Results.Json(new { error = "CURSEFORGE_API_KEY is not set" }, statusCode: 500);

    var result = await manager.StartCurseForgeAsync(request.Slug, request.FileId, curseForgeApiKey, request.Arguments ?? [], logger);

    return result;
});

application.MapPost("/stop-client", async (ClientProcessManager manager) =>
{
    var stopped = await manager.StopAsync();

    return stopped
        ? Results.Ok(new { status = "stopped" })
        : Results.NotFound(new { error = "no client is running" });
});

application.MapPost("/send-chat", async (HttpContext context, ClientProcessManager manager, ILogger<ClientProcessManager> logger) =>
{
    var request = await context.Request.ReadFromJsonAsync<SendChatRequest>();

    if (request is null || string.IsNullOrWhiteSpace(request.Message))
        return Results.BadRequest(new { error = "message is required" });

    if (!manager.IsRunning)
        return Results.Conflict(new { error = "no client is running" });

    var result = await ChatSender.SendAsync(request.Message, logger);

    return result;
});

application.MapGet("/screen", async (ClientProcessManager manager) =>
{
    if (!manager.IsRunning)
        return Results.Conflict(new { error = "no client is running" });

    var screenshot = await ScreenCapture.CaptureAsync();

    return screenshot;
});

application.Run();

// --- Request models ---

public sealed record StartVanillaRequest(
    [property: JsonPropertyName("version")] string? Version,
    [property: JsonPropertyName("arguments")] string[]? Arguments);

public sealed record StartCurseForgeRequest(
    [property: JsonPropertyName("slug")] string? Slug,
    [property: JsonPropertyName("fileId")] int FileId,
    [property: JsonPropertyName("arguments")] string[]? Arguments);

public sealed record SendChatRequest(
    [property: JsonPropertyName("message")] string? Message);

// --- ClientProcessManager ---

public sealed class ClientProcessManager
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private Process? _clientProcess;

    public bool IsRunning => _clientProcess is not null && !_clientProcess.HasExited;

    public async Task<IResult> StartAsync(string portableMinecraftVersion, string[] arguments, ILogger logger)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (IsRunning)
                return Results.Conflict(new { error = "a client is already running" });

            await EnsureDisplayAsync(logger);

            var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft";
            var processArguments = new List<string> { "--main-dir", minecraftDirectory, "start", portableMinecraftVersion };
            processArguments.AddRange(arguments);

            logger.LogInformation("Launching portablemc with version: {Version}", portableMinecraftVersion);

            var startInfo = new ProcessStartInfo
            {
                FileName = "portablemc",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var argument in processArguments)
                startInfo.ArgumentList.Add(argument);

            _clientProcess = Process.Start(startInfo);

            return Results.Ok(new { status = "started", version = portableMinecraftVersion, pid = _clientProcess?.Id });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IResult> StartCurseForgeAsync(string slug, int fileId, string apiKey, string[] arguments, ILogger logger)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (IsRunning)
                return Results.Conflict(new { error = "a client is already running" });

            var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft";
            Directory.CreateDirectory(minecraftDirectory);

            var modpackMarkerFile = Path.Combine(minecraftDirectory, ".curseforge-modpack");
            var portableMinecraftVersionFile = Path.Combine(minecraftDirectory, ".curseforge-portablemc-version");
            var requestedMarker = $"{slug} {fileId}";
            var existingMarker = File.Exists(modpackMarkerFile) ? (await File.ReadAllTextAsync(modpackMarkerFile)).Trim() : "";
            string portableMinecraftVersion;

            if (existingMarker == requestedMarker)
            {
                if (!File.Exists(portableMinecraftVersionFile))
                    return Results.Json(new { error = "PortableMC version cache file is missing" }, statusCode: 500);

                portableMinecraftVersion = (await File.ReadAllTextAsync(portableMinecraftVersionFile)).Trim();
                logger.LogInformation("Using existing installation in {Directory}", minecraftDirectory);
            }
            else
            {
                portableMinecraftVersion = await InstallCurseForgeModpackAsync(slug, fileId, apiKey, minecraftDirectory, modpackMarkerFile, portableMinecraftVersionFile, logger);
            }

            await EnsureDisplayAsync(logger);

            var processArguments = new List<string> { "--main-dir", minecraftDirectory, "start", portableMinecraftVersion };
            processArguments.AddRange(arguments);

            logger.LogInformation("Launching portablemc with version: {Version}", portableMinecraftVersion);

            var startInfo = new ProcessStartInfo
            {
                FileName = "portablemc",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var argument in processArguments)
                startInfo.ArgumentList.Add(argument);

            _clientProcess = Process.Start(startInfo);

            return Results.Ok(new { status = "started", version = portableMinecraftVersion, pid = _clientProcess?.Id });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> StopAsync()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (_clientProcess is null || _clientProcess.HasExited)
            {
                _clientProcess = null;
                return false;
            }

            _clientProcess.Kill(entireProcessTree: false);

            try
            {
                await _clientProcess.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException)
            {
                _clientProcess.Kill(entireProcessTree: true);
                await _clientProcess.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }

            _clientProcess = null;
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static async Task EnsureDisplayAsync(ILogger logger)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":99";
        Environment.SetEnvironmentVariable("DISPLAY", display);

        var checkInfo = new ProcessStartInfo
        {
            FileName = "xdpyinfo",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        checkInfo.ArgumentList.Add("-display");
        checkInfo.ArgumentList.Add(display);

        using var checkProcess = Process.Start(checkInfo);

        if (checkProcess is not null)
        {
            await checkProcess.WaitForExitAsync();

            if (checkProcess.ExitCode == 0)
                return;
        }

        logger.LogInformation("Starting virtual display: {Display}", display);

        var lockFile = $"/tmp/.X{display.TrimStart(':')}-lock";

        if (File.Exists(lockFile))
            File.Delete(lockFile);

        var xvfbInfo = new ProcessStartInfo
        {
            FileName = "Xvfb",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        xvfbInfo.ArgumentList.Add(display);
        xvfbInfo.ArgumentList.Add("-screen");
        xvfbInfo.ArgumentList.Add("0");
        xvfbInfo.ArgumentList.Add("1280x720x24");

        Process.Start(xvfbInfo);

        for (var attempt = 0; attempt < 100; attempt++)
        {
            await Task.Delay(100);

            var retryCheckInfo = new ProcessStartInfo
            {
                FileName = "xdpyinfo",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            retryCheckInfo.ArgumentList.Add("-display");
            retryCheckInfo.ArgumentList.Add(display);

            using var retryCheck = Process.Start(retryCheckInfo);

            if (retryCheck is not null)
            {
                await retryCheck.WaitForExitAsync();

                if (retryCheck.ExitCode == 0)
                    break;
            }
        }

        var xfwm4Info = new ProcessStartInfo
        {
            FileName = "xfwm4",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process.Start(xfwm4Info);
    }

    private static async Task<string> InstallCurseForgeModpackAsync(string slug, int fileId, string apiKey, string minecraftDirectory, string modpackMarkerFile, string portableMinecraftVersionFile, ILogger logger)
    {
        const int minecraftGameId = 432;
        const int batchSize = 50;

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

        logger.LogInformation("Resolving CurseForge project: {Slug}", slug);

        var searchUrl = $"https://api.curseforge.com/v1/mods/search?gameId={minecraftGameId}&slug={Uri.EscapeDataString(slug)}";
        var searchResponse = await httpClient.GetStringAsync(searchUrl);
        var searchDocument = JsonDocument.Parse(searchResponse);
        int? projectId = null;

        foreach (var mod in searchDocument.RootElement.GetProperty("data").EnumerateArray())
        {
            if (mod.GetProperty("slug").GetString() == slug)
            {
                projectId = mod.GetProperty("id").GetInt32();
                break;
            }
        }

        if (projectId is null)
            throw new InvalidOperationException($"CurseForge modpack was not found: {slug}");

        logger.LogInformation("Downloading modpack archive for project {ProjectId}", projectId);

        var downloadUrl = $"https://www.curseforge.com/api/v1/mods/{projectId}/files/{fileId}/download";
        var modpackBytes = await httpClient.GetByteArrayAsync(downloadUrl);
        var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            var modpackZipPath = Path.Combine(temporaryDirectory, "modpack.zip");
            await File.WriteAllBytesAsync(modpackZipPath, modpackBytes);

            logger.LogInformation("Reading modpack manifest");

            var extractInfo = new ProcessStartInfo
            {
                FileName = "unzip",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            extractInfo.ArgumentList.Add("-p");
            extractInfo.ArgumentList.Add(modpackZipPath);
            extractInfo.ArgumentList.Add("manifest.json");

            using var extractProcess = Process.Start(extractInfo) ?? throw new InvalidOperationException("Failed to start unzip");
            var manifestContent = await extractProcess.StandardOutput.ReadToEndAsync();
            await extractProcess.WaitForExitAsync();

            if (extractProcess.ExitCode != 0)
                throw new InvalidOperationException("Failed to read manifest.json from modpack archive");

            var manifest = JsonDocument.Parse(manifestContent);

            var modsDirectory = Path.Combine(minecraftDirectory, "mods");
            var resourcePacksDirectory = Path.Combine(minecraftDirectory, "resourcepacks");
            var shaderPacksDirectory = Path.Combine(minecraftDirectory, "shaderpacks");

            if (Directory.Exists(modsDirectory))
                Directory.Delete(modsDirectory, true);

            if (Directory.Exists(resourcePacksDirectory))
                Directory.Delete(resourcePacksDirectory, true);

            if (Directory.Exists(shaderPacksDirectory))
                Directory.Delete(shaderPacksDirectory, true);

            Directory.CreateDirectory(modsDirectory);
            Directory.CreateDirectory(resourcePacksDirectory);
            Directory.CreateDirectory(shaderPacksDirectory);

            if (manifest.RootElement.TryGetProperty("overrides", out var overridesElement))
            {
                var overridesDirectoryName = overridesElement.GetString();

                if (!string.IsNullOrEmpty(overridesDirectoryName))
                {
                    logger.LogInformation("Installing modpack overrides");

                    var overridesPath = Path.Combine(temporaryDirectory, "overrides");
                    Directory.CreateDirectory(overridesPath);

                    var overridesExtractInfo = new ProcessStartInfo
                    {
                        FileName = "unzip",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    overridesExtractInfo.ArgumentList.Add("-q");
                    overridesExtractInfo.ArgumentList.Add("-o");
                    overridesExtractInfo.ArgumentList.Add(modpackZipPath);
                    overridesExtractInfo.ArgumentList.Add($"{overridesDirectoryName}/*");
                    overridesExtractInfo.ArgumentList.Add("-d");
                    overridesExtractInfo.ArgumentList.Add(overridesPath);

                    using var overridesProcess = Process.Start(overridesExtractInfo);

                    if (overridesProcess is not null)
                        await overridesProcess.WaitForExitAsync();

                    CopyDirectoryRecursive(Path.Combine(overridesPath, overridesDirectoryName), minecraftDirectory);
                }
            }

            var requiredFileIds = new List<int>();

            if (manifest.RootElement.TryGetProperty("files", out var filesArray))
            {
                foreach (var file in filesArray.EnumerateArray())
                {
                    if (file.TryGetProperty("required", out var requiredProperty) && requiredProperty.ValueKind == JsonValueKind.False)
                        continue;

                    if (file.TryGetProperty("fileID", out var fileIdProperty))
                        requiredFileIds.Add(fileIdProperty.GetInt32());
                    else if (file.TryGetProperty("fileId", out var fileIdProperty2))
                        requiredFileIds.Add(fileIdProperty2.GetInt32());
                }
            }

            if (requiredFileIds.Count > 0)
            {
                logger.LogInformation("Resolving {Count} CurseForge files", requiredFileIds.Count);

                var fileMetadataList = new List<JsonElement>();

                for (var batchIndex = 0; batchIndex < requiredFileIds.Count; batchIndex += batchSize)
                {
                    var batch = requiredFileIds.Skip(batchIndex).Take(batchSize).ToArray();
                    var batchRequest = JsonSerializer.Serialize(new { fileIds = batch });
                    var batchContent = new StringContent(batchRequest, Encoding.UTF8, "application/json");
                    var batchResponse = await httpClient.PostAsync("https://api.curseforge.com/v1/mods/files", batchContent);
                    var batchResponseContent = await batchResponse.Content.ReadAsStringAsync();
                    var batchDocument = JsonDocument.Parse(batchResponseContent);

                    foreach (var item in batchDocument.RootElement.GetProperty("data").EnumerateArray())
                        fileMetadataList.Add(item.Clone());
                }

                var totalCount = fileMetadataList.Count;
                var currentIndex = 0;

                foreach (var fileMeta in fileMetadataList)
                {
                    currentIndex++;
                    var modId = fileMeta.GetProperty("modId").GetInt32();
                    var metaFileId = fileMeta.GetProperty("id").GetInt32();
                    var fileName = fileMeta.GetProperty("fileName").GetString() ?? "unknown";
                    var fileDownloadUrl = fileMeta.TryGetProperty("downloadUrl", out var downloadUrlElement) && downloadUrlElement.ValueKind == JsonValueKind.String
                        ? downloadUrlElement.GetString()
                        : null;

                    var targetDirectory = modsDirectory;

                    if (fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        if (fileName.Contains("shader", StringComparison.OrdinalIgnoreCase))
                            targetDirectory = shaderPacksDirectory;
                        else
                            targetDirectory = resourcePacksDirectory;
                    }

                    var safeFileName = Path.GetFileName(fileName);

                    if (string.IsNullOrEmpty(safeFileName) || safeFileName == "." || safeFileName == ".." || safeFileName != fileName || safeFileName.Contains('/'))
                        throw new InvalidOperationException($"Unexpected CurseForge file name '{fileName}' for mod id '{modId}' and file id '{metaFileId}'");

                    var targetFile = Path.Combine(targetDirectory, safeFileName);

                    if (File.Exists(targetFile))
                    {
                        logger.LogInformation("[{Current}/{Total}] Already exists: {FileName}", currentIndex, totalCount, safeFileName);
                        continue;
                    }

                    logger.LogInformation("[{Current}/{Total}] Downloading: {FileName}", currentIndex, totalCount, safeFileName);

                    byte[] fileBytes;

                    if (!string.IsNullOrEmpty(fileDownloadUrl))
                    {
                        fileBytes = await httpClient.GetByteArrayAsync(fileDownloadUrl);
                    }
                    else
                    {
                        fileBytes = await httpClient.GetByteArrayAsync($"https://www.curseforge.com/api/v1/mods/{modId}/files/{metaFileId}/download");
                    }

                    await File.WriteAllBytesAsync(targetFile, fileBytes);
                }
            }
            else
            {
                logger.LogInformation("No CurseForge files to download");
            }

            logger.LogInformation("Resolving PortableMC version");

            var minecraftVersion = manifest.RootElement.GetProperty("minecraft").GetProperty("version").GetString()
                ?? throw new InvalidOperationException("minecraft.version is missing from manifest");

            var portableMinecraftVersion = $"mojang:{minecraftVersion}";

            if (manifest.RootElement.GetProperty("minecraft").TryGetProperty("modLoaders", out var modLoadersElement))
            {
                foreach (var loader in modLoadersElement.EnumerateArray())
                {
                    if (loader.TryGetProperty("primary", out var primaryProperty) && primaryProperty.GetBoolean())
                    {
                        var modLoaderId = loader.GetProperty("id").GetString() ?? "";

                        if (modLoaderId.StartsWith("neoforge-"))
                        {
                            portableMinecraftVersion = $"neoforge::{modLoaderId["neoforge-".Length..]}";
                        }
                        else if (modLoaderId.StartsWith("forge-"))
                        {
                            var forgeVersion = modLoaderId["forge-".Length..];

                            portableMinecraftVersion = forgeVersion.StartsWith($"{minecraftVersion}-")
                                ? $"forge::{forgeVersion}"
                                : $"forge::{minecraftVersion}-{forgeVersion}";
                        }
                        else if (modLoaderId.StartsWith("fabric-"))
                        {
                            portableMinecraftVersion = $"fabric:{minecraftVersion}:{modLoaderId["fabric-".Length..]}";
                        }
                        else if (modLoaderId.StartsWith("quilt-"))
                        {
                            portableMinecraftVersion = $"quilt:{minecraftVersion}:{modLoaderId["quilt-".Length..]}";
                        }
                        else if (!string.IsNullOrEmpty(modLoaderId))
                        {
                            throw new InvalidOperationException($"Unsupported mod loader for CurseForge modpack '{slug}' (file id: {fileId}): {modLoaderId}");
                        }

                        break;
                    }
                }
            }

            await File.WriteAllTextAsync(portableMinecraftVersionFile, portableMinecraftVersion + "\n");
            await File.WriteAllTextAsync(modpackMarkerFile, $"{slug} {fileId}\n");
            logger.LogInformation("Installation marker updated");

            return portableMinecraftVersion;
        }
        finally
        {
            try
            {
                Directory.Delete(temporaryDirectory, true);
            }
            catch
            {
                // Best-effort cleanup
            }
        }
    }

    private static void CopyDirectoryRecursive(string sourceDirectory, string targetDirectory)
    {
        Directory.CreateDirectory(targetDirectory);

        foreach (var file in Directory.GetFiles(sourceDirectory))
        {
            var destinationFile = Path.Combine(targetDirectory, Path.GetFileName(file));
            File.Copy(file, destinationFile, overwrite: true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDirectory))
        {
            var destinationDirectory = Path.Combine(targetDirectory, Path.GetFileName(directory));
            CopyDirectoryRecursive(directory, destinationDirectory);
        }
    }
}

// --- ChatSender ---

public static class ChatSender
{
    public static async Task<IResult> SendAsync(string message, ILogger logger)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":99";

        var windowId = await FindLargestVisibleWindowAsync(display);

        if (string.IsNullOrEmpty(windowId))
            return Results.Json(new { error = "No visible Minecraft window found" }, statusCode: 500);

        await FocusWindowAsync(windowId);

        var chatOpened = await OpenChatAsync(windowId);

        if (!chatOpened)
            return Results.Json(new { error = "Failed to open chat interface" }, statusCode: 500);

        await TypeMessageAsync(windowId, message);
        await SendKeyAsync(windowId, "Return");

        logger.LogInformation("Successfully sent chat message");

        return Results.Ok(new { status = "sent" });
    }

    private static async Task<string?> FindLargestVisibleWindowAsync(string display)
    {
        var searchInfo = new ProcessStartInfo
        {
            FileName = "xdotool",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        searchInfo.ArgumentList.Add("search");
        searchInfo.ArgumentList.Add("--onlyvisible");
        searchInfo.ArgumentList.Add("--name");
        searchInfo.ArgumentList.Add(".*");
        searchInfo.EnvironmentVariables["DISPLAY"] = display;

        using var searchProcess = Process.Start(searchInfo);

        if (searchProcess is null)
            return null;

        var output = await searchProcess.StandardOutput.ReadToEndAsync();
        await searchProcess.WaitForExitAsync();

        var windowIds = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string? largestWindowId = null;
        long largestArea = 0;

        foreach (var candidateId in windowIds)
        {
            var geometryInfo = new ProcessStartInfo
            {
                FileName = "xdotool",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            geometryInfo.ArgumentList.Add("getwindowgeometry");
            geometryInfo.ArgumentList.Add("--shell");
            geometryInfo.ArgumentList.Add(candidateId.Trim());
            geometryInfo.EnvironmentVariables["DISPLAY"] = display;

            using var geometryProcess = Process.Start(geometryInfo);

            if (geometryProcess is null)
                continue;

            var geometryOutput = await geometryProcess.StandardOutput.ReadToEndAsync();
            await geometryProcess.WaitForExitAsync();

            if (geometryProcess.ExitCode != 0)
                continue;

            int width = 0, height = 0;

            foreach (var line in geometryOutput.Split('\n'))
            {
                if (line.StartsWith("WIDTH="))
                    int.TryParse(line["WIDTH=".Length..], out width);
                else if (line.StartsWith("HEIGHT="))
                    int.TryParse(line["HEIGHT=".Length..], out height);
            }

            var area = (long)width * height;

            if (area > largestArea)
            {
                largestArea = area;
                largestWindowId = candidateId.Trim();
            }
        }

        return largestWindowId;
    }

    private static async Task FocusWindowAsync(string windowId)
    {
        var focusInfo = new ProcessStartInfo
        {
            FileName = "xdotool",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        focusInfo.ArgumentList.Add("windowfocus");
        focusInfo.ArgumentList.Add(windowId);

        using var process = Process.Start(focusInfo);

        if (process is not null)
            await process.WaitForExitAsync();
    }

    private static async Task<bool> OpenChatAsync(string windowId)
    {
        const int brightnessThreshold = 5;
        const int maximumPresses = 100;

        var baselineBrightness = await CaptureBottomLeftBrightnessAsync(windowId);
        var currentPressCount = 0;
        var isChatOpen = false;

        while (currentPressCount < maximumPresses && !isChatOpen)
        {
            await SendKeyAsync(windowId, "t");
            currentPressCount++;

            await Task.Delay(132);

            var currentBrightness = await CaptureBottomLeftBrightnessAsync(windowId);
            var brightnessDifference = baselineBrightness - currentBrightness;

            if (Math.Abs(brightnessDifference) > brightnessThreshold)
            {
                isChatOpen = true;
                break;
            }
        }

        if (!isChatOpen)
            return false;

        for (var backspaceIndex = 0; backspaceIndex < currentPressCount; backspaceIndex++)
        {
            await SendKeyWithDelayAsync(windowId, "BackSpace", 50);
        }

        return true;
    }

    private static async Task<int> CaptureBottomLeftBrightnessAsync(string windowId)
    {
        var captureInfo = new ProcessStartInfo
        {
            FileName = "/bin/sh",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        captureInfo.ArgumentList.Add("-c");
        captureInfo.ArgumentList.Add($"import -window {windowId} miff:- | convert - -gravity SouthWest -crop 100x20+0+0 +repage -format \"%[fx:int(mean*100)]\" info:");

        using var process = Process.Start(captureInfo);

        if (process is null)
            return 0;

        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return int.TryParse(output.Trim(), out var brightness) ? brightness : 0;
    }

    private static async Task SendKeyAsync(string windowId, string key)
    {
        var keyInfo = new ProcessStartInfo
        {
            FileName = "xdotool",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        keyInfo.ArgumentList.Add("key");
        keyInfo.ArgumentList.Add("--clearmodifiers");
        keyInfo.ArgumentList.Add("--window");
        keyInfo.ArgumentList.Add(windowId);
        keyInfo.ArgumentList.Add(key);

        using var process = Process.Start(keyInfo);

        if (process is not null)
            await process.WaitForExitAsync();
    }

    private static async Task SendKeyWithDelayAsync(string windowId, string key, int delayMilliseconds)
    {
        var keyInfo = new ProcessStartInfo
        {
            FileName = "xdotool",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        keyInfo.ArgumentList.Add("key");
        keyInfo.ArgumentList.Add("--clearmodifiers");
        keyInfo.ArgumentList.Add("--window");
        keyInfo.ArgumentList.Add(windowId);
        keyInfo.ArgumentList.Add("--delay");
        keyInfo.ArgumentList.Add(delayMilliseconds.ToString());
        keyInfo.ArgumentList.Add(key);

        using var process = Process.Start(keyInfo);

        if (process is not null)
            await process.WaitForExitAsync();
    }

    private static async Task TypeMessageAsync(string windowId, string message)
    {
        var typeInfo = new ProcessStartInfo
        {
            FileName = "xdotool",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        typeInfo.ArgumentList.Add("type");
        typeInfo.ArgumentList.Add("--clearmodifiers");
        typeInfo.ArgumentList.Add("--window");
        typeInfo.ArgumentList.Add(windowId);
        typeInfo.ArgumentList.Add("--delay");
        typeInfo.ArgumentList.Add("50");
        typeInfo.ArgumentList.Add("--");
        typeInfo.ArgumentList.Add(message);

        using var process = Process.Start(typeInfo);

        if (process is not null)
            await process.WaitForExitAsync();
    }
}

// --- ScreenCapture ---

public static class ScreenCapture
{
    public static async Task<IResult> CaptureAsync()
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":99";
        var windowId = await FindLargestVisibleWindowAsync(display);

        if (string.IsNullOrEmpty(windowId))
            return Results.Json(new { error = "No visible window found" }, statusCode: 500);

        var captureInfo = new ProcessStartInfo
        {
            FileName = "import",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        captureInfo.ArgumentList.Add("-window");
        captureInfo.ArgumentList.Add(windowId);
        captureInfo.ArgumentList.Add("png:-");
        captureInfo.EnvironmentVariables["DISPLAY"] = display;

        using var process = Process.Start(captureInfo);

        if (process is null)
            return Results.Json(new { error = "Failed to start screen capture" }, statusCode: 500);

        using var memoryStream = new MemoryStream();
        await process.StandardOutput.BaseStream.CopyToAsync(memoryStream);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            return Results.Json(new { error = "Screen capture failed" }, statusCode: 500);

        return Results.File(memoryStream.ToArray(), "image/png");
    }

    private static async Task<string?> FindLargestVisibleWindowAsync(string display)
    {
        var searchInfo = new ProcessStartInfo
        {
            FileName = "xdotool",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        searchInfo.ArgumentList.Add("search");
        searchInfo.ArgumentList.Add("--onlyvisible");
        searchInfo.ArgumentList.Add("--name");
        searchInfo.ArgumentList.Add(".*");
        searchInfo.EnvironmentVariables["DISPLAY"] = display;

        using var searchProcess = Process.Start(searchInfo);

        if (searchProcess is null)
            return null;

        var output = await searchProcess.StandardOutput.ReadToEndAsync();
        await searchProcess.WaitForExitAsync();

        var windowIds = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string? largestWindowId = null;
        long largestArea = 0;

        foreach (var candidateId in windowIds)
        {
            var geometryInfo = new ProcessStartInfo
            {
                FileName = "xdotool",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            geometryInfo.ArgumentList.Add("getwindowgeometry");
            geometryInfo.ArgumentList.Add("--shell");
            geometryInfo.ArgumentList.Add(candidateId.Trim());
            geometryInfo.EnvironmentVariables["DISPLAY"] = display;

            using var geometryProcess = Process.Start(geometryInfo);

            if (geometryProcess is null)
                continue;

            var geometryOutput = await geometryProcess.StandardOutput.ReadToEndAsync();
            await geometryProcess.WaitForExitAsync();

            if (geometryProcess.ExitCode != 0)
                continue;

            int width = 0, height = 0;

            foreach (var line in geometryOutput.Split('\n'))
            {
                if (line.StartsWith("WIDTH="))
                    int.TryParse(line["WIDTH=".Length..], out width);
                else if (line.StartsWith("HEIGHT="))
                    int.TryParse(line["HEIGHT=".Length..], out height);
            }

            var area = (long)width * height;

            if (area > largestArea)
            {
                largestArea = area;
                largestWindowId = candidateId.Trim();
            }
        }

        return largestWindowId;
    }
}
