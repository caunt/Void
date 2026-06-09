#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net11.0
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using File = System.IO.File;

const string defaultMinecraftDirectory = "/root/.minecraft";
const string defaultCurseForgeApiBaseUrl = "https://api.curseforge.com";
const string defaultDisplay = ":99";
const string displayScreenWidth = "854";
const string displayScreenHeight = "480";
const string displayScreenResolution = $"{displayScreenWidth}x{displayScreenHeight}";
const string displayScreenDepth = "24";
const string displayScreen = $"{displayScreenResolution}x{displayScreenDepth}";
const int minecraftGameId = 432;
const int curseForgeFilesBatchSize = 50;
const int brightnessThreshold = 5;
const int maximumChatOpenPresses = 100;
const int displayReadyMaxAttempts = 50;
const int displayReadyDelayMilliseconds = 100;
const int displayProbeTimeoutMilliseconds = 1000;
const int externalProcessTimeoutMilliseconds = 5000;
const int screenCaptureTimeoutMilliseconds = 3000;
const int processStopTimeoutMilliseconds = 10000;
const int criticalProcessEarlyExitMilliseconds = 1000;

var builder = WebApplication.CreateBuilder(args);
var application = builder.Build();
var clientProcess = (Process?)null;
var clientState = ClientState.Idle;
var clientStateLock = new object();
var currentOperationId = 0;
var currentOperationName = (string?)null;
var currentOperationCancellationTokenSource = (CancellationTokenSource?)null;
var lastError = (string?)null;
var stateUpdatedAt = DateTimeOffset.UtcNow;
var chatOperationRunning = false;
var criticalProcesses = new HashSet<Process>();
var criticalProcessesLock = new object();
var expectedExitProcessIds = new HashSet<int>();
var expectedExitLock = new object();

application.Lifetime.ApplicationStopping.Register(StopCriticalProcesses);

application.MapGet("/health", () => "ok");

application.MapGet("/status", () => Results.Ok(CreateStatusBody("ok")));

application.MapGet("/start-vanilla", (HttpContext httpContext, string? version) =>
{
    if (string.IsNullOrWhiteSpace(version))
        return Results.BadRequest("version is required");

    var portableMinecraftArguments = httpContext.Request.Query["argument"].OfType<string>().ToArray();
    var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? defaultMinecraftDirectory;
    var portablemcVersion = $"mojang:{version}";
    var operationName = $"start-vanilla:{version}";
    var cancellationTokenSource = CreateOperationCancellationTokenSource();
    int operationId;

    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (!CanStartClientLocked())
        {
            cancellationTokenSource.Dispose();
            return Results.Conflict(CreateStatusBodyLocked("conflict", "a client is already running or changing state"));
        }

        operationId = BeginOperationLocked(operationName, ClientState.Starting, cancellationTokenSource);
    }

    RunDetachedOperation(operationId, operationName, cancellationTokenSource, cancellationToken => StartVanillaOperationAsync(operationId, minecraftDirectory, portablemcVersion, portableMinecraftArguments, cancellationToken));

    return Results.Ok(CreateStatusBody("starting"));
});

application.MapGet("/start-curseforge", (HttpContext httpContext, string? slug, int fileId) =>
{
    if (string.IsNullOrWhiteSpace(slug) || fileId <= 0)
        return Results.BadRequest("slug and positive fileId are required");

    var curseForgeApiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");

    if (string.IsNullOrWhiteSpace(curseForgeApiKey))
        return Results.Problem("CURSEFORGE_API_KEY is not set");

    Uri curseForgeApiBaseUri;

    try
    {
        curseForgeApiBaseUri = CreateCurseForgeApiBaseUri();
    }
    catch (InvalidOperationException exception)
    {
        return Results.Problem(exception.Message);
    }

    var portableMinecraftArguments = httpContext.Request.Query["argument"].OfType<string>().ToArray();
    var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? defaultMinecraftDirectory;
    var operationName = $"start-curseforge:{slug}:{fileId}";
    var cancellationTokenSource = CreateOperationCancellationTokenSource();
    int operationId;

    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (!CanStartClientLocked())
        {
            cancellationTokenSource.Dispose();
            return Results.Conflict(CreateStatusBodyLocked("conflict", "a client is already running or changing state"));
        }

        operationId = BeginOperationLocked(operationName, ClientState.Starting, cancellationTokenSource);
    }

    RunDetachedOperation(operationId, operationName, cancellationTokenSource, cancellationToken => StartCurseForgeOperationAsync(operationId, slug, fileId, curseForgeApiKey, curseForgeApiBaseUri, minecraftDirectory, portableMinecraftArguments, cancellationToken));

    return Results.Ok(CreateStatusBody("starting"));
});

application.MapGet("/stop-client", () =>
{
    var cancellationTokenSource = CreateOperationCancellationTokenSource();
    var operationName = "stop-client";
    Process? processToStop;
    int operationId;

    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (!HasActiveClientOrOperationLocked())
        {
            cancellationTokenSource.Dispose();
            clientState = ClientState.Idle;
            stateUpdatedAt = DateTimeOffset.UtcNow;
            return Results.NotFound("no client is running");
        }

        currentOperationCancellationTokenSource?.Cancel();
        processToStop = clientProcess;

        if (processToStop is not null && !processToStop.HasExited)
            MarkExpectedExit(processToStop);

        operationId = BeginOperationLocked(operationName, ClientState.Stopping, cancellationTokenSource);
    }

    RunDetachedOperation(operationId, operationName, cancellationTokenSource, cancellationToken => StopClientOperationAsync(operationId, processToStop, cancellationToken));

    return Results.Ok(CreateStatusBody("stopping"));
});

application.MapGet("/send-chat", (string? message) =>
{
    if (string.IsNullOrWhiteSpace(message))
        return Results.BadRequest("message is required");

    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (clientState != ClientState.Running || clientProcess is null || clientProcess.HasExited)
            return Results.Conflict(CreateStatusBodyLocked("conflict", "no client is running"));

        if (chatOperationRunning)
            return Results.Conflict(CreateStatusBodyLocked("conflict", "a chat operation is already running"));

        chatOperationRunning = true;
        lastError = null;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }

    RunDetachedChatOperation(message);

    return Results.Ok(CreateStatusBody("queued"));
});

application.MapGet("/screen", async () =>
{
    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (clientState != ClientState.Running || clientProcess is null || clientProcess.HasExited)
            return Results.Conflict("no client is running");
    }

    using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(application.Lifetime.ApplicationStopping);
    cancellationTokenSource.CancelAfter(screenCaptureTimeoutMilliseconds);

    try
    {
        var imageBytes = await CaptureScreenAsync(cancellationTokenSource.Token);
        return Results.File(imageBytes, "image/png");
    }
    catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
    {
        return Results.Problem("screen capture timed out");
    }
    catch (TimeoutException exception)
    {
        return Results.Problem(exception.Message);
    }
    catch (Exception exception)
    {
        return Results.Problem(exception.Message);
    }
});

application.Run();

async Task StartVanillaOperationAsync(int operationId, string minecraftDirectory, string portablemcVersion, string[] portableMinecraftArguments, CancellationToken cancellationToken)
{
    await PrepareDisplayAndWindowAsync(cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();

    Console.Error.WriteLine($"Launching Minecraft with PortableMC version: {portablemcVersion}");
    var process = LaunchPortableMinecraftClient(minecraftDirectory, portablemcVersion, portableMinecraftArguments, cancellationToken);

    CompleteStartOperation(operationId, process);
}

async Task StartCurseForgeOperationAsync(int operationId, string slug, int fileId, string curseForgeApiKey, Uri curseForgeApiBaseUri, string minecraftDirectory, string[] portableMinecraftArguments, CancellationToken cancellationToken)
{
    await PrepareDisplayAndWindowAsync(cancellationToken);
    Directory.CreateDirectory(minecraftDirectory);

    var markerFile = Path.Combine(minecraftDirectory, ".curseforge-modpack");
    var versionFile = Path.Combine(minecraftDirectory, ".curseforge-portablemc-version");
    var marker = $"{slug} {fileId}";
    var existingMarker = File.Exists(markerFile) ? (await File.ReadAllTextAsync(markerFile, cancellationToken)).Trim() : "";
    string portablemcVersion;

    Console.Error.WriteLine($"Starting CurseForge modpack '{slug}' file '{fileId}'");

    if (existingMarker == marker)
    {
        if (!File.Exists(versionFile))
            throw new InvalidOperationException($"PortableMC version cache file is missing: {versionFile}");

        portablemcVersion = (await File.ReadAllTextAsync(versionFile, cancellationToken)).Trim();

        Console.Error.WriteLine($"Using existing installation in {minecraftDirectory}");
    }
    else
    {
        portablemcVersion = await InstallModpack(slug, fileId, curseForgeApiKey, curseForgeApiBaseUri, minecraftDirectory, cancellationToken);

        await File.WriteAllTextAsync(versionFile, portablemcVersion, cancellationToken);
        await File.WriteAllTextAsync(markerFile, marker, cancellationToken);

        Console.Error.WriteLine("Installation marker updated");
    }

    cancellationToken.ThrowIfCancellationRequested();

    Console.Error.WriteLine($"Launching Minecraft with PortableMC version: {portablemcVersion}");
    var process = LaunchPortableMinecraftClient(minecraftDirectory, portablemcVersion, portableMinecraftArguments, cancellationToken);

    CompleteStartOperation(operationId, process);
}

async Task StopClientOperationAsync(int operationId, Process? processToStop, CancellationToken cancellationToken)
{
    if (processToStop is not null)
        await StopProcessAsync(processToStop, cancellationToken);

    lock (clientStateLock)
    {
        if (currentOperationId != operationId)
            return;

        clientProcess = null;
        currentOperationName = null;
        currentOperationCancellationTokenSource = null;
        clientState = ClientState.Idle;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }
}

async Task SendChatOperationAsync(string message, CancellationToken cancellationToken)
{
    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? defaultDisplay;
    var windowId = await FindLargestWindow(display, cancellationToken);

    if (windowId is null)
        throw new InvalidOperationException("no visible window found");

    await ResizeWindowToDisplayAsync(windowId, cancellationToken);
    await RunOrThrow(cancellationToken, "xdotool", "windowfocus", windowId);
    var chatOpened = await OpenChatAsync(windowId, display, cancellationToken);

    if (!chatOpened)
        throw new InvalidOperationException("failed to open chat interface after maximum attempts");

    await RunOrThrow(cancellationToken, "xdotool", "type", "--clearmodifiers", "--window", windowId, "--delay", "50", "--", message);
    await RunOrThrow(cancellationToken, "xdotool", "key", "--clearmodifiers", "--window", windowId, "Return");
}

Process LaunchPortableMinecraftClient(string directory, string version, string?[]? portableMinecraftArguments = null, CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();
    portableMinecraftArguments ??= [];
    var requestedPortableMinecraftArguments = portableMinecraftArguments.OfType<string>().ToArray();

    var process = StartCriticalProcess("portablemc", processInfo =>
    {
        processInfo.ArgumentList.Add("--main-dir");
        processInfo.ArgumentList.Add(directory);
        processInfo.ArgumentList.Add("start");
        processInfo.ArgumentList.Add(version);

        if (!HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--resolution"))
        {
            processInfo.ArgumentList.Add("--resolution");
            processInfo.ArgumentList.Add(displayScreenResolution);
        }

        foreach (var argument in requestedPortableMinecraftArguments)
            processInfo.ArgumentList.Add(argument);
    });

    if (process.WaitForExit(criticalProcessEarlyExitMilliseconds))
        Environment.FailFast($"portablemc exited immediately with code {process.ExitCode}");

    return process;
}

bool HasPortableMinecraftArgument(IEnumerable<string> arguments, string argumentName)
{
    return arguments.Any(argument => string.Equals(argument, argumentName, StringComparison.Ordinal) || argument.StartsWith($"{argumentName}=", StringComparison.Ordinal));
}

Uri CreateCurseForgeApiBaseUri()
{
    var configuredBaseUrl = Environment.GetEnvironmentVariable("CURSEFORGE_API_BASE_URL");
    var baseUrl = string.IsNullOrWhiteSpace(configuredBaseUrl)
        ? defaultCurseForgeApiBaseUrl
        : configuredBaseUrl.Trim();

    if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri)
        || baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps)
    {
        throw new InvalidOperationException("CURSEFORGE_API_BASE_URL must be an absolute HTTP or HTTPS URL");
    }

    var path = baseUri.AbsolutePath.TrimEnd('/');

    return new UriBuilder(baseUri)
    {
        Path = string.IsNullOrEmpty(path) ? "/" : path + "/",
        Query = "",
        Fragment = ""
    }.Uri;
}

async Task PrepareDisplayAndWindowAsync(CancellationToken cancellationToken)
{
    await EnsureDisplay(cancellationToken);

    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? defaultDisplay;
    var existingWindowId = await FindLargestWindow(display, cancellationToken);

    if (existingWindowId is not null)
        throw new InvalidOperationException("a client window is already running");
}

async Task<byte[]> CaptureScreenAsync(CancellationToken cancellationToken)
{
    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? defaultDisplay;
    var windowId = await FindLargestWindow(display, cancellationToken);

    if (windowId is null)
        throw new InvalidOperationException("no visible window found");

    await ResizeWindowToDisplayAsync(windowId, cancellationToken);

    var captureProcessInfo = CreateProcessInfo("import", ["-window", windowId, "png:-"], display: display);
    var captureResult = await RunProcessBytesAsync(captureProcessInfo, TimeSpan.FromMilliseconds(screenCaptureTimeoutMilliseconds), cancellationToken);

    return captureResult.ExitCode is not 0
        ? throw new InvalidOperationException($"screen capture failed: {captureResult.StandardError}")
        : captureResult.StandardOutput;
}

async Task ResizeWindowToDisplayAsync(string windowId, CancellationToken cancellationToken = default)
{
    await RunOrThrow(cancellationToken, "xdotool", "windowmove", "--sync", windowId, "0", "0");
    await RunOrThrow(cancellationToken, "xdotool", "windowsize", "--sync", windowId, displayScreenWidth, displayScreenHeight);
}

async Task EnsureDisplay(CancellationToken cancellationToken = default)
{
    var display = Environment.GetEnvironmentVariable("DISPLAY");

    if (!string.IsNullOrEmpty(display) && await IsDisplayReadyAsync(display, cancellationToken))
        return;

    display ??= defaultDisplay;
    Environment.SetEnvironmentVariable("DISPLAY", display);

    var displayNumber = display.TrimStart(':');
    var lockFile = $"/tmp/.X{displayNumber}-lock";

    if (File.Exists(lockFile))
        File.Delete(lockFile);

    StartCriticalProcess("Xvfb", processInfo =>
    {
        processInfo.ArgumentList.Add(display);
        processInfo.ArgumentList.Add("-screen");
        processInfo.ArgumentList.Add("0");
        processInfo.ArgumentList.Add(displayScreen);
    });

    var displayIsReady = false;

    for (var attempt = 0; attempt < displayReadyMaxAttempts; attempt++)
    {
        await Task.Delay(displayReadyDelayMilliseconds, cancellationToken);

        if (await IsDisplayReadyAsync(display, cancellationToken))
        {
            displayIsReady = true;
            break;
        }
    }

    if (!displayIsReady)
        throw new InvalidOperationException($"display {display} did not become ready after {displayReadyMaxAttempts} attempts");
}

async Task<bool> IsDisplayReadyAsync(string display, CancellationToken cancellationToken)
{
    try
    {
        var processInfo = CreateProcessInfo("xdpyinfo", ["-display", display], display: display);
        var result = await RunProcessTextAsync(processInfo, TimeSpan.FromMilliseconds(displayProbeTimeoutMilliseconds), cancellationToken);

        return result.ExitCode == 0;
    }
    catch (TimeoutException)
    {
        return false;
    }
}

async Task<bool> OpenChatAsync(string windowId, string display, CancellationToken cancellationToken = default)
{
    // Just in case, ensure chat window is closed
    await RunOrThrow(cancellationToken, "xdotool", "key", "--clearmodifiers", "--window", windowId, "Return");

    var baselineBrightness = await CaptureBrightnessAsync(windowId, display, cancellationToken);
    var currentPressCount = 0;

    while (currentPressCount < maximumChatOpenPresses)
    {
        await RunOrThrow(cancellationToken, "xdotool", "key", "--clearmodifiers", "--window", windowId, "t");
        currentPressCount++;

        await Task.Delay(132, cancellationToken);

        var currentBrightness = await CaptureBrightnessAsync(windowId, display, cancellationToken);
        var brightnessDifference = Math.Abs(baselineBrightness - currentBrightness);

        if (brightnessDifference <= brightnessThreshold)
            continue;

        for (var backspaceIndex = 0; backspaceIndex < currentPressCount; backspaceIndex++)
            await RunOrThrow(cancellationToken, "xdotool", "key", "--clearmodifiers", "--window", windowId, "--delay", "50", "BackSpace");

        return true;
    }

    return false;
}

async Task<int> CaptureBrightnessAsync(string windowId, string display, CancellationToken cancellationToken = default)
{
    var importProcessInfo = CreateProcessInfo("import", ["-window", windowId, "miff:-"], display: display);
    var importResult = await RunProcessBytesAsync(importProcessInfo, TimeSpan.FromMilliseconds(externalProcessTimeoutMilliseconds), cancellationToken);

    if (importResult.ExitCode != 0)
        throw new InvalidOperationException($"import exited with code {importResult.ExitCode} during brightness capture: {importResult.StandardError}");

    var convertProcessInfo = CreateProcessInfo("convert",
    [
        "-",
        "-gravity",
        "SouthWest",
        "-crop",
        "100x20+0+0",
        "+repage",
        "-format",
        "%[fx:int(mean*100)]",
        "info:"
    ], redirectStandardInput: true);

    var convertResult = await RunProcessWithInputAsync(convertProcessInfo, importResult.StandardOutput, TimeSpan.FromMilliseconds(externalProcessTimeoutMilliseconds), cancellationToken);
    var brightnessOutput = convertResult.StandardOutput;

    return convertResult.ExitCode != 0
        ? throw new InvalidOperationException($"convert exited with code {convertResult.ExitCode} during brightness capture: {convertResult.StandardError}")
        : !int.TryParse(brightnessOutput.Trim(), out var brightness)
            ? throw new InvalidOperationException($"failed to parse brightness value from convert output: '{brightnessOutput.Trim()}'")
            : brightness;
}

async Task<string?> FindLargestWindow(string display, CancellationToken cancellationToken = default)
{
    var searchProcessInfo = CreateProcessInfo("xdotool", ["search", "--onlyvisible", "--name", ".*"], display: display);
    var searchResult = await RunProcessTextAsync(searchProcessInfo, TimeSpan.FromMilliseconds(displayProbeTimeoutMilliseconds), cancellationToken);

    if (searchResult.ExitCode != 0)
        return null;

    string? largestWindowId = null;
    long largestArea = 0;

    foreach (var candidateWindowId in searchResult.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
    {
        var trimmedCandidateId = candidateWindowId.Trim();
        var nameProcessInfo = CreateProcessInfo("xdotool", ["getwindowname", trimmedCandidateId], display: display);
        var nameResult = await RunProcessTextAsync(nameProcessInfo, TimeSpan.FromMilliseconds(displayProbeTimeoutMilliseconds), cancellationToken);

        if (nameResult.ExitCode != 0 || string.IsNullOrWhiteSpace(nameResult.StandardOutput))
            continue;

        var geometryProcessInfo = CreateProcessInfo("xdotool", ["getwindowgeometry", "--shell", trimmedCandidateId], display: display);
        var geometryResult = await RunProcessTextAsync(geometryProcessInfo, TimeSpan.FromMilliseconds(displayProbeTimeoutMilliseconds), cancellationToken);

        if (geometryResult.ExitCode != 0)
            continue;

        int width = 0, height = 0;

        foreach (var line in geometryResult.StandardOutput.Split('\n'))
        {
            if (line.StartsWith("WIDTH=") && int.TryParse(line["WIDTH=".Length..], out var widthValue))
                width = widthValue;
            else if (line.StartsWith("HEIGHT=") && int.TryParse(line["HEIGHT=".Length..], out var heightValue))
                height = heightValue;
        }

        if ((long)width * height > largestArea)
        {
            largestArea = (long)width * height;
            largestWindowId = trimmedCandidateId;
        }
    }

    return largestWindowId;
}

async Task RunOrThrow(CancellationToken cancellationToken, params string[] command)
{
    var processInfo = CreateProcessInfo(command[0], command[1..]);
    var result = await RunProcessTextAsync(processInfo, TimeSpan.FromMilliseconds(externalProcessTimeoutMilliseconds), cancellationToken);

    if (result.ExitCode is not 0)
        throw new InvalidOperationException($"{command[0]} exited with code {result.ExitCode}: {result.StandardError}");
}

ProcessStartInfo CreateProcessInfo(string fileName, IEnumerable<string> arguments, string? display = null, bool redirectStandardInput = false)
{
    var processInfo = new ProcessStartInfo(fileName)
    {
        UseShellExecute = false,
        RedirectStandardInput = redirectStandardInput,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };

    if (display is not null)
        processInfo.Environment["DISPLAY"] = display;

    foreach (var argument in arguments)
        processInfo.ArgumentList.Add(argument);

    return processInfo;
}

async Task<ProcessTextResult> RunProcessTextAsync(ProcessStartInfo processInfo, TimeSpan timeout, CancellationToken cancellationToken)
{
    using var process = Process.Start(processInfo)
                        ?? throw new InvalidOperationException($"failed to start {processInfo.FileName}");

    var standardOutputTask = process.StandardOutput.ReadToEndAsync();
    var standardErrorTask = process.StandardError.ReadToEndAsync();

    try
    {
        await WaitForProcessExitAsync(process, processInfo.FileName, timeout, cancellationToken);
    }
    catch
    {
        await IgnoreTaskAsync(standardOutputTask);
        await IgnoreTaskAsync(standardErrorTask);
        throw;
    }

    return new ProcessTextResult(process.ExitCode, await standardOutputTask, await standardErrorTask);
}

async Task<ProcessBytesResult> RunProcessBytesAsync(ProcessStartInfo processInfo, TimeSpan timeout, CancellationToken cancellationToken)
{
    using var process = Process.Start(processInfo)
                        ?? throw new InvalidOperationException($"failed to start {processInfo.FileName}");

    using var standardOutput = new MemoryStream();
    var standardOutputTask = process.StandardOutput.BaseStream.CopyToAsync(standardOutput);
    var standardErrorTask = process.StandardError.ReadToEndAsync();

    try
    {
        await WaitForProcessExitAsync(process, processInfo.FileName, timeout, cancellationToken);
        await standardOutputTask;
    }
    catch
    {
        await IgnoreTaskAsync(standardOutputTask);
        await IgnoreTaskAsync(standardErrorTask);
        throw;
    }

    return new ProcessBytesResult(process.ExitCode, standardOutput.ToArray(), await standardErrorTask);
}

async Task<ProcessTextResult> RunProcessWithInputAsync(ProcessStartInfo processInfo, byte[] standardInput, TimeSpan timeout, CancellationToken cancellationToken)
{
    using var process = Process.Start(processInfo)
                        ?? throw new InvalidOperationException($"failed to start {processInfo.FileName}");

    var standardInputTask = WriteProcessInputAsync(process, standardInput, cancellationToken);
    var standardOutputTask = process.StandardOutput.ReadToEndAsync();
    var standardErrorTask = process.StandardError.ReadToEndAsync();

    try
    {
        await WaitForProcessExitAsync(process, processInfo.FileName, timeout, cancellationToken);

        try
        {
            await standardInputTask;
        }
        catch (IOException) when (process.HasExited)
        {
            // The process can reject stdin before producing a non-zero exit code.
        }
    }
    catch
    {
        await IgnoreTaskAsync(standardInputTask);
        await IgnoreTaskAsync(standardOutputTask);
        await IgnoreTaskAsync(standardErrorTask);
        throw;
    }

    return new ProcessTextResult(process.ExitCode, await standardOutputTask, await standardErrorTask);
}

async Task WriteProcessInputAsync(Process process, byte[] standardInput, CancellationToken cancellationToken)
{
    try
    {
        await process.StandardInput.BaseStream.WriteAsync(standardInput, cancellationToken);
    }
    finally
    {
        process.StandardInput.Close();
    }
}

async Task WaitForProcessExitAsync(Process process, string processName, TimeSpan timeout, CancellationToken cancellationToken)
{
    using var timeoutSource = new CancellationTokenSource(timeout);
    using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

    try
    {
        await process.WaitForExitAsync(linkedCancellationTokenSource.Token);
    }
    catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
    {
        KillProcess(process);
        await WaitForKilledProcessAsync(process);
        throw new TimeoutException($"{processName} timed out after {timeout.TotalSeconds:F1} seconds");
    }
    catch (OperationCanceledException)
    {
        KillProcess(process);
        await WaitForKilledProcessAsync(process);
        throw;
    }
}

async Task WaitForKilledProcessAsync(Process process)
{
    try
    {
        await process.WaitForExitAsync(CancellationToken.None);
    }
    catch (InvalidOperationException) when (process.HasExited)
    {
        // Intentionally left blank
    }
}

async Task IgnoreTaskAsync(Task task)
{
    try
    {
        await task;
    }
    catch
    {
        // Best-effort cleanup after the process has already failed or timed out.
    }
}

async Task StopProcessAsync(Process process, CancellationToken cancellationToken)
{
    try
    {
        if (process.HasExited)
            return;

        MarkExpectedExit(process);
        process.Kill(entireProcessTree: true);

        await WaitForProcessExitAsync(process, process.ProcessName, TimeSpan.FromMilliseconds(processStopTimeoutMilliseconds), cancellationToken);
    }
    catch (InvalidOperationException) when (process.HasExited)
    {
        // Intentionally left blank
    }
}

void KillProcess(Process process)
{
    try
    {
        if (!process.HasExited)
            process.Kill(entireProcessTree: true);
    }
    catch (InvalidOperationException) when (process.HasExited)
    {
        // Intentionally left blank
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Failed to kill process {process.ProcessName}: {exception}");
    }
}

CancellationTokenSource CreateOperationCancellationTokenSource()
{
    return CancellationTokenSource.CreateLinkedTokenSource(application.Lifetime.ApplicationStopping);
}

void RunDetachedOperation(int operationId, string operationName, CancellationTokenSource cancellationTokenSource, Func<CancellationToken, Task> operation)
{
    _ = Task.Run(async () =>
    {
        try
        {
            await operation(cancellationTokenSource.Token);
        }
        catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested || application.Lifetime.ApplicationStopping.IsCancellationRequested)
        {
            CompleteCanceledOperation(operationId);
        }
        catch (Exception exception)
        {
            FailOperation(operationId, operationName, exception);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }
    });
}

void RunDetachedChatOperation(string message)
{
    var cancellationTokenSource = CreateOperationCancellationTokenSource();

    _ = Task.Run(async () =>
    {
        try
        {
            await SendChatOperationAsync(message, cancellationTokenSource.Token);

            lock (clientStateLock)
            {
                lastError = null;
                stateUpdatedAt = DateTimeOffset.UtcNow;
            }
        }
        catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested || application.Lifetime.ApplicationStopping.IsCancellationRequested)
        {
            // The application is stopping or the chat operation was canceled.
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"send-chat failed: {exception}");

            lock (clientStateLock)
            {
                lastError = exception.Message;
                stateUpdatedAt = DateTimeOffset.UtcNow;
            }
        }
        finally
        {
            lock (clientStateLock)
            {
                chatOperationRunning = false;
                stateUpdatedAt = DateTimeOffset.UtcNow;
            }

            cancellationTokenSource.Dispose();
        }
    });
}

int BeginOperationLocked(string operationName, ClientState state, CancellationTokenSource cancellationTokenSource)
{
    currentOperationId++;
    currentOperationName = operationName;
    currentOperationCancellationTokenSource = cancellationTokenSource;
    clientState = state;
    lastError = null;
    stateUpdatedAt = DateTimeOffset.UtcNow;

    return currentOperationId;
}

bool CanStartClientLocked()
{
    return (clientState is ClientState.Idle or ClientState.Failed) && !IsClientProcessRunningLocked();
}

bool HasActiveClientOrOperationLocked()
{
    return (clientState is ClientState.Starting or ClientState.Running or ClientState.Stopping) || IsClientProcessRunningLocked();
}

bool IsClientProcessRunningLocked()
{
    return clientProcess is not null && !clientProcess.HasExited;
}

void RefreshClientStateLocked()
{
    if (clientProcess is null || !clientProcess.HasExited)
        return;

    clientProcess = null;

    if (clientState is ClientState.Running)
    {
        clientState = ClientState.Idle;
        currentOperationName = null;
        currentOperationCancellationTokenSource = null;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }
}

void CompleteStartOperation(int operationId, Process process)
{
    var shouldStopProcess = false;

    lock (clientStateLock)
    {
        if (currentOperationId != operationId || clientState is not ClientState.Starting)
        {
            shouldStopProcess = true;
        }
        else
        {
            clientProcess = process;
            clientState = ClientState.Running;
            currentOperationName = null;
            currentOperationCancellationTokenSource = null;
            lastError = null;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    if (!shouldStopProcess)
        return;

    MarkExpectedExit(process);
    KillProcess(process);
}

void CompleteCanceledOperation(int operationId)
{
    lock (clientStateLock)
    {
        if (currentOperationId != operationId)
            return;

        currentOperationName = null;
        currentOperationCancellationTokenSource = null;
        clientState = IsClientProcessRunningLocked() ? ClientState.Running : ClientState.Idle;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }
}

void FailOperation(int operationId, string operationName, Exception exception)
{
    Console.Error.WriteLine($"{operationName} failed: {exception}");

    lock (clientStateLock)
    {
        if (currentOperationId != operationId)
            return;

        currentOperationName = null;
        currentOperationCancellationTokenSource = null;
        lastError = exception.Message;
        clientState = IsClientProcessRunningLocked() ? ClientState.Running : ClientState.Failed;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }
}

ApiStatus CreateStatusBody(string status, string? message = null)
{
    lock (clientStateLock)
    {
        RefreshClientStateLocked();
        return CreateStatusBodyLocked(status, message);
    }
}

ApiStatus CreateStatusBodyLocked(string status, string? message = null)
{
    var pid = IsClientProcessRunningLocked() ? clientProcess?.Id : null;

    return new ApiStatus(status, clientState.ToString().ToLowerInvariant(), currentOperationId, currentOperationName, pid, message, lastError, stateUpdatedAt);
}

void MarkExpectedExit(Process process)
{
    lock (expectedExitLock)
        expectedExitProcessIds.Add(process.Id);
}

void StopCriticalProcesses()
{
    Process[] processes;

    lock (criticalProcessesLock)
        processes = criticalProcesses.ToArray();

    foreach (var process in processes)
    {
        if (process.HasExited)
            continue;

        try
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
        }
        catch (InvalidOperationException) when (process.HasExited)
        {
            // Intentionally left blank
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Failed to stop process {process.ProcessName}: {exception}");
        }
    }
}

Process StartCriticalProcess(string fileName, Action<ProcessStartInfo> configure)
{
    var processInfo = new ProcessStartInfo(fileName) { UseShellExecute = false };
    configure(processInfo);

    var process = Process.Start(processInfo)
                  ?? throw new InvalidOperationException($"failed to start critical process '{fileName}'");

    lock (criticalProcessesLock)
        criticalProcesses.Add(process);

    process.EnableRaisingEvents = true;
    process.Exited += (sender, eventArguments) =>
    {
        lock (criticalProcessesLock)
            criticalProcesses.Remove(process);

        if (application.Lifetime.ApplicationStopping.IsCancellationRequested)
            return;

        lock (expectedExitLock)
        {
            if (expectedExitProcessIds.Remove(process.Id))
                return;
        }

        Environment.FailFast($"{fileName} exited unexpectedly with code {process.ExitCode}");
    };

    if (process.HasExited)
    {
        lock (expectedExitLock)
        {
            if (!expectedExitProcessIds.Contains(process.Id))
                Environment.FailFast($"{fileName} exited unexpectedly with code {process.ExitCode}");
        }
    }

    return process;
}

void DeleteDirectoryIfExists(string path)
{
    if (Directory.Exists(path))
        Directory.Delete(path, recursive: true);
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
        || safeName.Any(char.IsControl))
    {
        throw new InvalidOperationException($"Unexpected CurseForge file name '{fileName}' for mod id '{modId}' and file id '{modFileId}'");
    }
}

string DetermineTargetDirectory(string fileName, string minecraftDirectory)
{
    return fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase)
        ? Path.Combine(minecraftDirectory, "mods")
        : fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
            ? fileName.Contains("shader", StringComparison.OrdinalIgnoreCase)
                ? Path.Combine(minecraftDirectory, "shaderpacks")
                : Path.Combine(minecraftDirectory, "resourcepacks")
            : Path.Combine(minecraftDirectory, "mods");
}

async Task<string> InstallModpack(string slug, int fileId, string apiKey, Uri apiBaseUri, string minecraftDirectory, CancellationToken cancellationToken)
{
    using var httpClient = new HttpClient();
    var curseForgeClient = new CurseForgeApiClient(httpClient, apiBaseUri, apiKey);

    Console.Error.WriteLine("Resolving CurseForge project");
    var searchResult = await curseForgeClient.SearchModsAsync(minecraftGameId, slug, cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();

    var project = searchResult.FirstOrDefault(modpack => modpack.Slug == slug)
                  ?? throw new InvalidOperationException($"modpack not found: {slug}");

    Console.Error.WriteLine("Downloading modpack archive");
    var archiveDownloadUrl = await ResolveDownloadUrlAsync(curseForgeClient, project.Id, fileId, cancellationToken);
    var archiveBytes = await DownloadWithFallbackAsync(httpClient, archiveDownloadUrl, apiKey, cancellationToken);

    Console.Error.WriteLine("Reading modpack manifest");
    await using var archive = new ZipArchive(new MemoryStream(archiveBytes), ZipArchiveMode.Read);
    var manifestEntry = archive.GetEntry("manifest.json")
                        ?? throw new InvalidOperationException("manifest.json not found");

    await using var manifestStream = manifestEntry.Open();
    var manifest = await JsonSerializer.DeserializeAsync<CurseForgeManifest>(manifestStream, new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = false }, cancellationToken)
                   ?? throw new InvalidOperationException("failed to deserialize manifest.json");

    cancellationToken.ThrowIfCancellationRequested();

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
            cancellationToken.ThrowIfCancellationRequested();

            if (!entry.FullName.StartsWith(overridesFolder + "/") || entry.FullName.Length <= overridesFolder.Length + 1)
                continue;

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
            if (file.Required is false)
                continue;

            var resolvedFileId = file.FileId ?? file.FileId;

            if (resolvedFileId is > 0)
            {
                requiredFileIds.Add(resolvedFileId.Value);
            }
        }

        if (requiredFileIds.Count > 0)
        {
            Console.Error.WriteLine($"Resolving {requiredFileIds.Count} CurseForge files");

            var allFileMetadata = new List<CurseForgeFile>();

            for (var batchStart = 0; batchStart < requiredFileIds.Count; batchStart += curseForgeFilesBatchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = requiredFileIds.Skip(batchStart).Take(curseForgeFilesBatchSize).ToList();
                var files = await curseForgeClient.GetFilesAsync(batch, cancellationToken);
                allFileMetadata.AddRange(files);
            }

            var totalFileCount = allFileMetadata.Count;
            var downloadIndex = 0;

            foreach (var fileMeta in allFileMetadata)
            {
                cancellationToken.ThrowIfCancellationRequested();

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

                var fileDownloadUrl = await ResolveModFileDownloadUrlAsync(curseForgeClient, fileMeta.ModId, fileMeta.Id, fileMeta.DownloadUrl, cancellationToken);
                var fileBytes = await DownloadWithFallbackAsync(httpClient, fileDownloadUrl, apiKey, cancellationToken);

                await File.WriteAllBytesAsync(destinationPath, fileBytes, cancellationToken);
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
                continue;

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
                Environment.FailFast($"Unsupported mod loader for CurseForge modpack '{slug}' (file id: {fileId}): {loaderId}");
            }

            break;
        }
    }

    return portablemcVersion;
}

async Task<string> ResolveDownloadUrlAsync(CurseForgeApiClient curseForgeClient, int projectId, int modFileId, CancellationToken cancellationToken)
{
    var file = await curseForgeClient.GetModFileAsync(projectId, modFileId, cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();

    if (!string.IsNullOrEmpty(file.DownloadUrl))
        return file.DownloadUrl;

    var downloadUrl = await curseForgeClient.GetModFileDownloadUrlAsync(projectId, modFileId, cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();

    return !string.IsNullOrEmpty(downloadUrl)
        ? downloadUrl
        : $"https://www.curseforge.com/api/v1/mods/{projectId}/files/{modFileId}/download";
}

async Task<string> ResolveModFileDownloadUrlAsync(CurseForgeApiClient curseForgeClient, int modId, int modFileId, string? sdkDownloadUrl, CancellationToken cancellationToken)
{
    if (!string.IsNullOrEmpty(sdkDownloadUrl))
        return sdkDownloadUrl;

    var downloadUrl = await curseForgeClient.GetModFileDownloadUrlAsync(modId, modFileId, cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();

    return !string.IsNullOrEmpty(downloadUrl)
        ? downloadUrl
        : $"https://www.curseforge.com/api/v1/mods/{modId}/files/{modFileId}/download";
}

async Task<byte[]> DownloadWithFallbackAsync(HttpClient httpClient, string downloadUrl, string apiKey, CancellationToken cancellationToken)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);

    if (downloadUrl.Contains("curseforge.com", StringComparison.OrdinalIgnoreCase))
        request.Headers.TryAddWithoutValidation("x-api-key", apiKey);

    using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsByteArrayAsync(cancellationToken);
}

sealed class CurseForgeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public CurseForgeApiClient(HttpClient httpClient, Uri baseUri, string apiKey)
    {
        _httpClient = httpClient;
        _baseUri = baseUri;
        _apiKey = apiKey;
    }

    public async Task<List<CurseForgeProject>> SearchModsAsync(int gameId, string slug, CancellationToken cancellationToken)
    {
        var slugQuery = Uri.EscapeDataString(slug);
        var response = await GetAsync<CurseForgeApiListResponse<CurseForgeProject>>($"v1/mods/search?gameId={gameId}&slug={slugQuery}", cancellationToken);

        return response.Data ?? [];
    }

    public async Task<CurseForgeFile> GetModFileAsync(int modId, int fileId, CancellationToken cancellationToken)
    {
        var response = await GetAsync<CurseForgeApiResponse<CurseForgeFile>>($"v1/mods/{modId}/files/{fileId}", cancellationToken);

        return response.Data ?? throw new InvalidOperationException($"CurseForge file not found: mod {modId}, file {fileId}");
    }

    public async Task<string?> GetModFileDownloadUrlAsync(int modId, int fileId, CancellationToken cancellationToken)
    {
        var response = await GetAsync<CurseForgeApiResponse<string?>>($"v1/mods/{modId}/files/{fileId}/download-url", cancellationToken);

        return response.Data;
    }

    public async Task<List<CurseForgeFile>> GetFilesAsync(List<int> fileIds, CancellationToken cancellationToken)
    {
        var response = await PostAsync<CurseForgeApiListResponse<CurseForgeFile>>("v1/mods/files", new CurseForgeFilesRequest(fileIds), cancellationToken);

        return response.Data ?? [];
    }

    private async Task<T> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_baseUri, relativeUrl));

        return await SendAsync<T>(request, cancellationToken);
    }

    private async Task<T> PostAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_baseUri, relativeUrl))
        {
            Content = JsonContent.Create(body, options: _jsonOptions)
        };

        return await SendAsync<T>(request, cancellationToken);
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.TryAddWithoutValidation("x-api-key", _apiKey);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken)
               ?? throw new InvalidOperationException("CurseForge API returned an empty response");
    }
}

record CurseForgeApiListResponse<T>(List<T>? Data);

record CurseForgeApiResponse<T>(T? Data);

record CurseForgeProject(int Id, string? Slug);

record CurseForgeFile(int Id, int ModId, string FileName, string? DownloadUrl);

record CurseForgeFilesRequest(List<int> FileIds);

record CurseForgeManifest(CurseForgeMinecraft? Minecraft, string? Overrides, List<CurseForgeManifestFile>? Files);

record CurseForgeMinecraft(string? Version, List<CurseForgeModLoader>? ModLoaders);

record CurseForgeModLoader(string? Id, bool? Primary);

record CurseForgeManifestFile([property: JsonPropertyName("fileID")] int? FileId, bool? Required);

record ApiStatus(string Status, string State, int OperationId, string? Operation, int? Pid, string? Message, string? Error, DateTimeOffset UpdatedAt);

record ProcessTextResult(int ExitCode, string StandardOutput, string StandardError);

record ProcessBytesResult(int ExitCode, byte[] StandardOutput, string StandardError);

enum ClientState
{
    Idle,
    Starting,
    Running,
    Stopping,
    Failed
}
