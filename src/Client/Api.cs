#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net11.0
#:property PublishAot=false

using System.Diagnostics;
using System.Globalization;
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
const string portableMinecraftLegacyJvmExecutablePath = "/opt/zulu-8-i686/bin/java";
const string portableMinecraftLegacyJvmPath = "/usr/local/bin/java-i686";
const string portableMinecraftArmLwjgl3Version = "3.3.3";
const string portableMinecraftArmLwjgl4Version = "3.4.1";
const string chatInputBrightnessCropGeometry = "854x2+0+451";
const int minecraftGameId = 432;
const int curseForgeFilesBatchSize = 50;
const int userInterfaceStableConfirmationCount = 2;
const double buttonHoverDifferenceRatioThreshold = 0.03;
const double serverAddressFieldDifferenceRatioThreshold = 0.01;
const double chatInputBrightnessRatioThreshold = 0.7;
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
var inputOperationRunning = false;
var chatInputTargetsWindow = true;
var chatInputSupportsVisualConfirmation = true;
var displayProcess = (Process?)null;
var criticalProcesses = new HashSet<Process>();
var criticalProcessesLock = new object();
var expectedExitProcessIds = new HashSet<int>();
var expectedExitLock = new object();

application.Lifetime.ApplicationStopping.Register(StopCriticalProcesses);

application.MapGet("/health", () => "ok");

application.MapGet("/status", () => Results.Ok(CreateStatusBody("ok")));

application.MapPut("/options", async Task<IResult> (HttpRequest request, CancellationToken cancellationToken) =>
{
    var minecraftDirectory = Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? defaultMinecraftDirectory;
    using var reader = new StreamReader(request.Body);
    var options = await reader.ReadToEndAsync(cancellationToken);

    _ = Directory.CreateDirectory(minecraftDirectory);
    await File.WriteAllTextAsync(Path.Combine(minecraftDirectory, "options.txt"), options, cancellationToken);

    return Results.Ok();
});

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

application.MapGet("/send-chat", async Task<IResult> (string? message, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(message))
        return Results.BadRequest("message is required");

    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (clientState != ClientState.Running || clientProcess is null || clientProcess.HasExited)
            return Results.Conflict(CreateStatusBodyLocked("conflict", "no client is running"));

        if (inputOperationRunning)
            return Results.Conflict(CreateStatusBodyLocked("conflict", "a client input operation is already running"));

        inputOperationRunning = true;
        lastError = null;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }

    using var operationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, application.Lifetime.ApplicationStopping);

    try
    {
        await SendChatOperationAsync(message, operationCancellationTokenSource.Token);

        lock (clientStateLock)
        {
            lastError = null;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }

        return Results.Ok(CreateStatusBody("sent"));
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"send-chat failed: {exception}");

        lock (clientStateLock)
        {
            lastError = exception.Message;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }

        return Results.Problem(exception.Message);
    }
    finally
    {
        lock (clientStateLock)
        {
            inputOperationRunning = false;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }
    }
});

application.MapGet("/join-server", async Task<IResult> (string? host, int port, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(host))
        return Results.BadRequest("host is required");

    if (port is < 1 or > 65535)
        return Results.BadRequest("port must be between 1 and 65535");

    lock (clientStateLock)
    {
        RefreshClientStateLocked();

        if (clientState != ClientState.Running || clientProcess is null || clientProcess.HasExited)
            return Results.Conflict(CreateStatusBodyLocked("conflict", "no client is running"));

        if (inputOperationRunning)
            return Results.Conflict(CreateStatusBodyLocked("conflict", "a client input operation is already running"));

        inputOperationRunning = true;
        lastError = null;
        stateUpdatedAt = DateTimeOffset.UtcNow;
    }

    using var operationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, application.Lifetime.ApplicationStopping);

    try
    {
        await JoinServerOperationAsync(host.Trim(), port, operationCancellationTokenSource.Token);

        lock (clientStateLock)
        {
            lastError = null;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }

        return Results.Ok(CreateStatusBody("joined"));
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"join-server failed: {exception}");

        lock (clientStateLock)
        {
            lastError = exception.Message;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }

        return Results.Problem(exception.Message);
    }
    finally
    {
        lock (clientStateLock)
        {
            inputOperationRunning = false;
            stateUpdatedAt = DateTimeOffset.UtcNow;
        }
    }
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

    var preferredInputWindowId = chatInputTargetsWindow ? windowId : null;
    await ResizeWindowToDisplayAsync(windowId, cancellationToken);
    await RunOrThrow(cancellationToken, "xdotool", "windowfocus", "--sync", windowId);
    await ResumeGameIfPausedAsync(windowId, display, cancellationToken);
    var inputWindowId = await OpenChatAsync(windowId, preferredInputWindowId, display, cancellationToken);
    await TypeTextAsync(inputWindowId, message, chatInputSupportsVisualConfirmation ? 50 : 150, cancellationToken);
    await SubmitChatAsync(windowId, inputWindowId, display, cancellationToken);
}

async Task JoinServerOperationAsync(string host, int port, CancellationToken cancellationToken)
{
    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? defaultDisplay;
    var windowId = await WaitForLargestWindowAsync(display, cancellationToken);
    var serverAddress = $"{host}:{port}";

    await ResizeWindowToDisplayAsync(windowId, cancellationToken);
    await RunOrThrow(cancellationToken, "xdotool", "windowfocus", windowId);

    await VisuallyClickButtonAsync("Multiplayer", windowId, display, screen => screen.TryFindMainMenuMultiplayerButton(out var button) ? button : null, cancellationToken);

    var nextScreen = await WaitForMultiplayerOrOnlinePlayWarningAsync(windowId, display, cancellationToken);

    if (nextScreen.Kind is NavigationScreenKind.OnlinePlayWarning)
    {
        Console.Error.WriteLine("Visually confirmed the third-party online play warning");
        await VisuallyClickButtonAsync("Proceed", windowId, display, screen => screen.TryFindOnlinePlayWarningProceedButton(out var button) ? button : null, cancellationToken);
        await WaitForScreenTargetAsync("multiplayer server list", windowId, display, screen => screen.TryFindMultiplayerScreenDirectConnectionButton(out var button) ? button : null, cancellationToken);
    }

    while (true)
    {
        await SubmitDirectConnectionAsync(windowId, display, serverAddress, cancellationToken);

        if (await WaitForInteractiveGameScreenAsync(windowId, display, cancellationToken))
            break;

        Console.Error.WriteLine("Visually confirmed that the connection failed; returning to the server list to retry");
        await VisuallyClickButtonAsync("Back to server list", windowId, display, screen => screen.TryFindConnectionFailureBackButton(out var button) ? button : null, cancellationToken);
        await WaitForScreenTargetAsync("multiplayer server list", windowId, display, screen => screen.TryFindMultiplayerScreenDirectConnectionButton(out var button) ? button : null, cancellationToken);
    }

    Console.Error.WriteLine($"Visually confirmed navigation from the main menu to server {serverAddress}");
}

async Task SubmitDirectConnectionAsync(string windowId, string display, string serverAddress, CancellationToken cancellationToken)
{
    await VisuallyClickButtonAsync("Direct Connection", windowId, display, screen => screen.TryFindMultiplayerScreenDirectConnectionButton(out var button) ? button : null, cancellationToken);
    await WaitForScreenTargetAsync("direct connection form", windowId, display, screen => screen.TryFindDirectConnectionScreen(out var directConnectionScreen) ? directConnectionScreen.JoinButton : null, cancellationToken);
    await EnterServerAddressAsync(windowId, display, serverAddress, cancellationToken);
    await VisuallyClickButtonAsync("Join Server", windowId, display, screen => screen.TryFindDirectConnectionScreen(out var directConnectionScreen) ? directConnectionScreen.JoinButton : null, cancellationToken);
    await WaitForDirectConnectionScreenToCloseAsync(windowId, display, cancellationToken);
}

async Task<bool> WaitForInteractiveGameScreenAsync(string windowId, string display, CancellationToken cancellationToken)
{
    var preferredInputWindowId = chatInputTargetsWindow ? windowId : null;
    var targetPreferredWindow = true;
    var useChatKey = true;
    ScreenRectangle? previousFailureButton = null;
    var failureConfirmationCount = 0;

    while (true)
    {
        var inputWindowId = targetPreferredWindow
            ? preferredInputWindowId
            : preferredInputWindowId is null ? windowId : null;
        targetPreferredWindow = !targetPreferredWindow;

        if (chatInputSupportsVisualConfirmation)
        {
            await PressKeyAsync(inputWindowId, "t", cancellationToken);
        }
        else
        {
            var chatKey = useChatKey ? "t" : "slash";
            useChatKey = !useChatKey;
            await PressKeySlowlyAsync(null, chatKey, cancellationToken);
        }

        if (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
        {
            await ClearChatInputAsync(pressKeysSlowly: !chatInputSupportsVisualConfirmation, cancellationToken: cancellationToken);

            if (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
                break;
        }

        using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

        if (screen.TryFindConnectionFailureBackButton(out var failureButton))
        {
            failureConfirmationCount = previousFailureButton is { } previous && AreMatchingRectangles(previous, failureButton)
                ? failureConfirmationCount + 1
                : 1;
            previousFailureButton = failureButton;

            if (failureConfirmationCount >= userInterfaceStableConfirmationCount)
                return false;
        }
        else
        {
            previousFailureButton = null;
            failureConfirmationCount = 0;
        }
    }

    if (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
        await PressKeyAsync(null, "Escape", cancellationToken);

    while (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
        cancellationToken.ThrowIfCancellationRequested();

    await ResumeGameIfPausedAsync(windowId, display, cancellationToken);

    Console.Error.WriteLine("Visually confirmed an interactive in-game screen");
    return true;
}

async Task ResumeGameIfPausedAsync(string windowId, string display, CancellationToken cancellationToken)
{
    using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

    if (!screen.TryFindPauseMenuBackToGameButton(out _))
        return;

    Console.Error.WriteLine("Visually confirmed the pause menu; returning to the game");
    await VisuallyClickButtonAsync("Back to Game", windowId, display, currentScreen => currentScreen.TryFindPauseMenuBackToGameButton(out var button) ? button : null, cancellationToken);

    while (true)
    {
        using var currentScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

        if (!currentScreen.TryFindPauseMenuBackToGameButton(out _))
            return;

    }
}

async Task<string> WaitForLargestWindowAsync(string display, CancellationToken cancellationToken)
{
    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var windowId = await FindLargestWindow(display, cancellationToken);

        if (windowId is not null)
            return windowId;

    }
}

async Task<NavigationScreenTarget> WaitForMultiplayerOrOnlinePlayWarningAsync(string windowId, string display, CancellationToken cancellationToken)
{
    await MoveMouseAsync(windowId, 2, 2, cancellationToken);

    NavigationScreenTarget? previousTarget = null;
    var stableConfirmationCount = 0;
    string? lastScreenDescription = null;

    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
            lastScreenDescription = screen.ToString();
            NavigationScreenTarget? target = null;

            if (screen.TryFindMultiplayerScreenDirectConnectionButton(out var directConnectionButton))
                target = new NavigationScreenTarget(NavigationScreenKind.MultiplayerServerList, directConnectionButton);
            else if (screen.TryFindOnlinePlayWarningProceedButton(out var proceedButton))
                target = new NavigationScreenTarget(NavigationScreenKind.OnlinePlayWarning, proceedButton);

            if (target is not null && previousTarget is { } previous && AreMatchingTargets(previous, target.Value))
                stableConfirmationCount++;
            else
                stableConfirmationCount = target is null ? 0 : 1;

            previousTarget = target;

            if (target is not null && stableConfirmationCount >= userInterfaceStableConfirmationCount)
                return target.Value;
        }
        catch (InvalidOperationException exception)
        {
            lastScreenDescription = exception.Message;
        }

        if (cancellationToken.IsCancellationRequested)
            throw new InvalidOperationException($"Failed to visually confirm the multiplayer server list or online play warning. Last screen: {lastScreenDescription}");
    }
}

async Task<ScreenRectangle> WaitForScreenTargetAsync(string screenName, string windowId, string display, Func<ScreenImage, ScreenRectangle?> locateTarget, CancellationToken cancellationToken)
{
    await MoveMouseAsync(windowId, 2, 2, cancellationToken);

    ScreenRectangle? previousTarget = null;
    var stableConfirmationCount = 0;
    string? lastScreenDescription = null;

    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
            lastScreenDescription = screen.ToString();
            var target = locateTarget(screen);

            if (target is not null && previousTarget is { } previous && AreMatchingRectangles(previous, target.Value))
                stableConfirmationCount++;
            else
                stableConfirmationCount = target is null ? 0 : 1;

            previousTarget = target;

            if (target is not null && stableConfirmationCount >= userInterfaceStableConfirmationCount)
            {
                Console.Error.WriteLine($"Visually confirmed {screenName}: {target.Value}");
                return target.Value;
            }
        }
        catch (InvalidOperationException exception)
        {
            lastScreenDescription = exception.Message;
        }

        if (cancellationToken.IsCancellationRequested)
            throw new InvalidOperationException($"Failed to visually confirm {screenName}. Last screen: {lastScreenDescription}");
    }
}

async Task VisuallyClickButtonAsync(string buttonName, string windowId, string display, Func<ScreenImage, ScreenRectangle?> locateButton, CancellationToken cancellationToken)
{
    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await MoveMouseAsync(windowId, 2, 2, cancellationToken);

        try
        {
            using var baselineScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
            var button = locateButton(baselineScreen);

            if (button is null)
                continue;

            await MoveMouseAsync(windowId, button.Value.CenterX, button.Value.CenterY, cancellationToken);
            var hoverConfirmationCount = 0;

            while (hoverConfirmationCount < userInterfaceStableConfirmationCount)
            {
                using var hoveredScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
                var differenceRatio = baselineScreen.CalculateDifferenceRatio(hoveredScreen, button.Value);

                if (differenceRatio < buttonHoverDifferenceRatioThreshold)
                {
                    hoverConfirmationCount = 0;
                    continue;
                }

                hoverConfirmationCount++;

                if (hoverConfirmationCount >= userInterfaceStableConfirmationCount)
                    Console.Error.WriteLine($"Visually confirmed {buttonName} hover at {button.Value} ({differenceRatio:P1} changed pixels)");
            }

            await ClickMouseAsync(cancellationToken);
            return;
        }
        catch (InvalidOperationException)
        {
            // The game can replace its window or screen while it is still loading. Reacquire the target.
        }
    }
}

async Task EnterServerAddressAsync(string windowId, string display, string serverAddress, CancellationToken cancellationToken)
{
    await MoveMouseAsync(windowId, 2, 2, cancellationToken);
    await WaitForScreenTargetAsync("direct connection form", windowId, display, screen => screen.TryFindDirectConnectionScreen(out var directConnectionScreen) ? directConnectionScreen.JoinButton : null, cancellationToken);

    var directConnectionResult = await WaitForDirectConnectionScreenImageAsync(windowId, display, cancellationToken);
    using var directConnectionScreenImage = directConnectionResult.Screen;
    var directConnectionScreen = directConnectionResult.DirectConnectionScreen;

    await MoveMouseAsync(windowId, directConnectionScreen.ServerAddressField.CenterX, directConnectionScreen.ServerAddressField.CenterY, cancellationToken);
    await ClickMouseAsync(cancellationToken);
    var emptyResult = await ClearServerAddressFieldAsync(windowId, display, cancellationToken);
    using var emptyScreen = emptyResult.Screen;
    var emptyDirectConnectionScreen = emptyResult.DirectConnectionScreen;

    var previousScreen = emptyScreen;
    var previousDirectConnectionScreen = emptyDirectConnectionScreen;

    foreach (var character in serverAddress)
    {
        await TypeTextWithoutDelayAsync(null, character.ToString(), cancellationToken);
        var characterResult = await WaitForServerAddressFieldChangeAsync(windowId, display, previousScreen, previousDirectConnectionScreen.ServerAddressField, 0, cancellationToken);

        if (!ReferenceEquals(previousScreen, emptyScreen))
            previousScreen.Dispose();

        previousScreen = characterResult.Screen;
        previousDirectConnectionScreen = characterResult.DirectConnectionScreen;
    }

    if (!ReferenceEquals(previousScreen, emptyScreen))
        previousScreen.Dispose();

    var enteredConfirmationCount = 0;

    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var enteredScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

        if (!enteredScreen.TryFindDirectConnectionScreen(out var enteredDirectConnectionScreen)
            || enteredScreen.IsServerAddressFieldEmpty(enteredDirectConnectionScreen.ServerAddressField))
        {
            enteredConfirmationCount = 0;
            continue;
        }

        var fieldDifferenceRatio = emptyScreen.CalculateDifferenceRatio(enteredScreen, emptyDirectConnectionScreen.ServerAddressField.Inset(3));

        if (fieldDifferenceRatio < serverAddressFieldDifferenceRatioThreshold)
        {
            enteredConfirmationCount = 0;
            continue;
        }

        enteredConfirmationCount++;

        if (enteredConfirmationCount >= userInterfaceStableConfirmationCount)
        {
            Console.Error.WriteLine($"Visually confirmed server address entry ({fieldDifferenceRatio:P1} field change)");
            return;
        }
    }
}

async Task<(ScreenImage Screen, DirectConnectionScreen DirectConnectionScreen)> ClearServerAddressFieldAsync(string windowId, string display, CancellationToken cancellationToken)
{
    ScreenImage? confirmedEmptyScreen = null;
    var emptyConfirmationCount = 0;

    try
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var screenResult = await WaitForDirectConnectionScreenImageAsync(windowId, display, cancellationToken);

            if (screenResult.Screen.IsServerAddressFieldEmpty(screenResult.DirectConnectionScreen.ServerAddressField))
            {
                emptyConfirmationCount++;
                confirmedEmptyScreen?.Dispose();
                confirmedEmptyScreen = screenResult.Screen;

                if (emptyConfirmationCount >= userInterfaceStableConfirmationCount)
                {
                    Console.Error.WriteLine("Visually confirmed an empty server address field");
                    return (confirmedEmptyScreen, screenResult.DirectConnectionScreen);
                }

                continue;
            }

            emptyConfirmationCount = 0;
            confirmedEmptyScreen?.Dispose();
            confirmedEmptyScreen = null;
            screenResult.Screen.Dispose();

            await PressKeyAsync(null, "End", cancellationToken);
            await PressKeyAsync(null, "BackSpace", cancellationToken);
        }
    }
    catch
    {
        confirmedEmptyScreen?.Dispose();
        throw;
    }
}

async Task<(ScreenImage Screen, DirectConnectionScreen DirectConnectionScreen)> WaitForServerAddressFieldChangeAsync(string windowId, string display, ScreenImage baselineScreen, ScreenRectangle baselineField, double minimumDifferenceRatio, CancellationToken cancellationToken)
{
    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

        if (screen.TryFindDirectConnectionScreen(out var directConnectionScreen)
            && baselineScreen.CalculateDifferenceRatio(screen, baselineField.Inset(3)) > minimumDifferenceRatio)
        {
            return (screen, directConnectionScreen);
        }

        screen.Dispose();
    }
}

async Task<(ScreenImage Screen, DirectConnectionScreen DirectConnectionScreen)> WaitForDirectConnectionScreenImageAsync(string windowId, string display, CancellationToken cancellationToken)
{
    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

        if (screen.TryFindDirectConnectionScreen(out var directConnectionScreen))
            return (screen, directConnectionScreen);

        screen.Dispose();
    }
}

async Task WaitForDirectConnectionScreenToCloseAsync(string windowId, string display, CancellationToken cancellationToken)
{
    var closedConfirmationCount = 0;

    while (closedConfirmationCount < userInterfaceStableConfirmationCount)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
        closedConfirmationCount = screen.TryFindDirectConnectionScreen(out _) ? 0 : closedConfirmationCount + 1;
    }

    Console.Error.WriteLine("Visually confirmed that the Join Server action closed the direct connection form");
}

async Task<ScreenImage> CaptureScreenImageAsync(string windowId, string display, CancellationToken cancellationToken)
{
    var captureProcessInfo = CreateProcessInfo("import", ["-window", windowId, "-depth", "8", "ppm:-"], display: display);
    var captureResult = await RunProcessBytesAsync(captureProcessInfo, TimeSpan.FromMilliseconds(screenCaptureTimeoutMilliseconds), cancellationToken);

    if (captureResult.ExitCode is not 0)
        throw new InvalidOperationException($"screen analysis capture failed: {captureResult.StandardError}");

    return ScreenImage.LoadPortablePixmap(captureResult.StandardOutput);
}

async Task MoveMouseAsync(string windowId, int x, int y, CancellationToken cancellationToken)
{
    await RunOrThrow(cancellationToken, "xdotool", "windowfocus", windowId);
    await RunOrThrow(cancellationToken, "xdotool", "mousemove", "--window", windowId, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture));
}

async Task ClickMouseAsync(CancellationToken cancellationToken)
{
    await RunOrThrow(cancellationToken, "xdotool", "click", "1");
}

bool AreMatchingTargets(NavigationScreenTarget left, NavigationScreenTarget right)
{
    return left.Kind == right.Kind && AreMatchingRectangles(left.Target, right.Target);
}

bool AreMatchingRectangles(ScreenRectangle left, ScreenRectangle right)
{
    return Math.Abs(left.Left - right.Left) <= 3
        && Math.Abs(left.Top - right.Top) <= 3
        && Math.Abs(left.Right - right.Right) <= 3
        && Math.Abs(left.Bottom - right.Bottom) <= 3;
}

Process LaunchPortableMinecraftClient(string directory, string version, string?[]? portableMinecraftArguments = null, CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();
    portableMinecraftArguments ??= [];
    var requestedPortableMinecraftArguments = portableMinecraftArguments.OfType<string>().ToArray();
    chatInputTargetsWindow = !UsesActiveWindowChatInput(version);
    chatInputSupportsVisualConfirmation = !RequiresBlindChatInput(version);

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

        if (File.Exists(portableMinecraftLegacyJvmExecutablePath) && version.StartsWith("mojang:", StringComparison.Ordinal))
        {
            if (UsesLegacyLwjgl(version) && !HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--jvm"))
            {
                processInfo.ArgumentList.Add("--jvm");
                processInfo.ArgumentList.Add(portableMinecraftLegacyJvmPath);
            }
            else if (!UsesLegacyLwjgl(version) && !HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--fix-lwjgl"))
            {
                processInfo.ArgumentList.Add("--fix-lwjgl");
                processInfo.ArgumentList.Add(GetArmLwjglVersion(version));
            }
        }

        foreach (var argument in requestedPortableMinecraftArguments)
            processInfo.ArgumentList.Add(argument);
    });

    if (process.WaitForExit(criticalProcessEarlyExitMilliseconds))
        Environment.FailFast($"portablemc exited immediately with code {process.ExitCode}");

    return process;
}

string GetArmLwjglVersion(string version)
{
    var versionComponents = version["mojang:".Length..].Split('.');

    if (versionComponents.Length >= 1 && int.TryParse(versionComponents[0], out var majorVersion) && majorVersion >= 26)
        return portableMinecraftArmLwjgl4Version;

    return portableMinecraftArmLwjgl3Version;
}

bool UsesLegacyLwjgl(string version)
{
    var versionComponents = version["mojang:".Length..].Split('.');

    return versionComponents.Length >= 2
        && int.TryParse(versionComponents[0], out var majorVersion)
        && int.TryParse(versionComponents[1], out var minorVersion)
        && majorVersion == 1
        && minorVersion <= 12;
}

bool UsesActiveWindowChatInput(string version)
{
    if (!version.StartsWith("mojang:", StringComparison.Ordinal))
        return false;

    var versionComponents = version["mojang:".Length..].Split('.');

    return versionComponents.Length >= 2
        && int.TryParse(versionComponents[0], out var majorVersion)
        && int.TryParse(versionComponents[1], out var minorVersion)
        && majorVersion == 1
        && minorVersion <= 12;
}

bool RequiresBlindChatInput(string version)
{
    if (!version.StartsWith("mojang:", StringComparison.Ordinal))
        return false;

    var versionComponents = version["mojang:".Length..].Split('.');

    return versionComponents.Length >= 2
        && int.TryParse(versionComponents[0], out var majorVersion)
        && int.TryParse(versionComponents[1], out var minorVersion)
        && majorVersion == 1
        && minorVersion == 7;
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

    while (true)
    {
        if (await FindLargestWindow(display, cancellationToken) is null)
            return;
    }
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
    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? defaultDisplay;
    Environment.SetEnvironmentVariable("DISPLAY", display);

    if (await IsDisplayReadyAsync(display, cancellationToken))
        return;

    if (displayProcess is not null && !displayProcess.HasExited)
    {
        await WaitForDisplayReadyAsync(display, cancellationToken);
        return;
    }

    var displayNumber = display.TrimStart(':');
    var lockFile = $"/tmp/.X{displayNumber}-lock";

    if (File.Exists(lockFile))
        File.Delete(lockFile);

    displayProcess = StartCriticalProcess("Xvfb", processInfo =>
    {
        processInfo.ArgumentList.Add(display);
        processInfo.ArgumentList.Add("-screen");
        processInfo.ArgumentList.Add("0");
        processInfo.ArgumentList.Add(displayScreen);
        processInfo.ArgumentList.Add("-noreset");
        processInfo.ArgumentList.Add("-nolisten");
        processInfo.ArgumentList.Add("tcp");
    });

    await WaitForDisplayReadyAsync(display, cancellationToken);
}

async Task WaitForDisplayReadyAsync(string display, CancellationToken cancellationToken)
{
    while (true)
    {
        if (await IsDisplayReadyAsync(display, cancellationToken))
            return;
    }
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

async Task TypeTextAsync(string? windowId, string text, int delayMilliseconds, CancellationToken cancellationToken = default)
{
    var delay = delayMilliseconds.ToString(CultureInfo.InvariantCulture);

    if (windowId is null)
        await RunOrThrow(cancellationToken, "xdotool", "type", "--clearmodifiers", "--delay", delay, "--", text);
    else
        await RunOrThrow(cancellationToken, "xdotool", "type", "--clearmodifiers", "--window", windowId, "--delay", delay, "--", text);
}

async Task TypeTextWithoutDelayAsync(string? windowId, string text, CancellationToken cancellationToken = default)
{
    if (windowId is null)
        await RunOrThrow(cancellationToken, "xdotool", "type", "--clearmodifiers", "--", text);
    else
        await RunOrThrow(cancellationToken, "xdotool", "type", "--clearmodifiers", "--window", windowId, "--", text);
}

async Task PressKeyAsync(string? windowId, string key, CancellationToken cancellationToken = default)
{
    if (windowId is null)
        await RunOrThrow(cancellationToken, "xdotool", "key", "--clearmodifiers", key);
    else
        await RunOrThrow(cancellationToken, "xdotool", "key", "--clearmodifiers", "--window", windowId, key);
}

async Task PressKeySlowlyAsync(string? windowId, string key, CancellationToken cancellationToken = default)
{
    if (windowId is null)
        await RunOrThrow(cancellationToken, "xdotool", "keydown", "--clearmodifiers", key);
    else
        await RunOrThrow(cancellationToken, "xdotool", "keydown", "--clearmodifiers", "--window", windowId, key);

    await Task.Delay(150, cancellationToken);

    if (windowId is null)
        await RunOrThrow(cancellationToken, "xdotool", "keyup", key);
    else
        await RunOrThrow(cancellationToken, "xdotool", "keyup", "--window", windowId, key);
}

async Task ClearChatInputAsync(bool pressKeysSlowly = false, CancellationToken cancellationToken = default)
{
    if (pressKeysSlowly)
    {
        await SelectAllTextAsync(cancellationToken);
        await PressKeySlowlyAsync(null, "BackSpace", cancellationToken);
    }
    else
        await ClearTextWithoutDelayAsync(cancellationToken);
}

async Task SelectAllTextAsync(CancellationToken cancellationToken)
{
    await RunOrThrow(cancellationToken, "xdotool", "keydown", "ctrl", "key", "a", "keyup", "ctrl");
}

async Task ClearTextWithoutDelayAsync(CancellationToken cancellationToken)
{
    await RunOrThrow(cancellationToken, "xdotool", "keydown", "ctrl", "key", "a", "keyup", "ctrl", "key", "BackSpace");
}

async Task<string?> OpenChatAsync(string windowId, string? preferredInputWindowId, string display, CancellationToken cancellationToken = default)
{
    if (!chatInputSupportsVisualConfirmation)
    {
        var useChatKey = true;

        while (true)
        {
            var chatKey = useChatKey ? "t" : "slash";
            useChatKey = !useChatKey;
            await PressKeySlowlyAsync(null, chatKey, cancellationToken);

            if (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            {
                await ClearChatInputAsync(pressKeysSlowly: true, cancellationToken: cancellationToken);

                if (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
                    return null;
            }
        }
    }

    while (true)
    {
        await PressKeyAsync(preferredInputWindowId, "t", cancellationToken);

        while (!await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            cancellationToken.ThrowIfCancellationRequested();

        await ClearChatInputAsync(cancellationToken: cancellationToken);

        if (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            return null;
    }
}

async Task SubmitChatAsync(string windowId, string? inputWindowId, string display, CancellationToken cancellationToken = default)
{
    if (!chatInputSupportsVisualConfirmation)
    {
        await PressKeyAsync(null, "Return", cancellationToken);
        return;
    }

    await PressKeyAsync(inputWindowId, "Return", cancellationToken);

    while (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
        cancellationToken.ThrowIfCancellationRequested();
}

async Task<bool> IsChatInputVisibleAsync(string windowId, string display, CancellationToken cancellationToken = default)
{
    var screenshotPath = Path.Combine(Path.GetTempPath(), $"portable-minecraft-chat-{Guid.NewGuid():N}.png");

    try
    {
        var importProcessInfo = CreateProcessInfo("import", ["-window", windowId, screenshotPath], display: display);
        var importResult = await RunProcessTextAsync(importProcessInfo, TimeSpan.FromMilliseconds(screenCaptureTimeoutMilliseconds), cancellationToken);

        if (importResult.ExitCode != 0)
            throw new InvalidOperationException($"chat input capture failed: {importResult.StandardError}");

        var convertProcessInfo = CreateProcessInfo("convert",
        [
            screenshotPath,
            "-colorspace",
            "Gray",
            "-crop",
            chatInputBrightnessCropGeometry,
            "+repage",
            "-scale",
            "1x2!",
            "-format",
            "%[fx:u.p{0,0}.r*100] %[fx:u.p{0,1}.r*100]",
            "info:"
        ]);
        var convertResult = await RunProcessTextAsync(convertProcessInfo, TimeSpan.FromMilliseconds(externalProcessTimeoutMilliseconds), cancellationToken);

        if (convertResult.ExitCode != 0)
            throw new InvalidOperationException($"chat input analysis failed: {convertResult.StandardError}");

        var brightnessValues = convertResult.StandardOutput.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (brightnessValues.Length != 2
            || !double.TryParse(brightnessValues[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var brightnessAboveInput)
            || !double.TryParse(brightnessValues[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var inputBrightness))
        {
            throw new InvalidOperationException($"failed to parse chat input brightness values: '{convertResult.StandardOutput.Trim()}'");
        }

        return brightnessAboveInput >= 1 && inputBrightness / brightnessAboveInput <= chatInputBrightnessRatioThreshold;
    }
    finally
    {
        if (File.Exists(screenshotPath))
            File.Delete(screenshotPath);
    }
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

ProcessStartInfo CreateProcessInfo(string fileName, IEnumerable<string> arguments, string? display = null)
{
    var processInfo = new ProcessStartInfo(fileName)
    {
        UseShellExecute = false,
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

sealed class ScreenImage : IDisposable
{
    private const int MinimumButtonWidth = 60;
    private const int MaximumButtonWidth = 430;
    private const int MinimumButtonHeight = 36;
    private const int MaximumButtonHeight = 42;
    private const byte BlackThreshold = 12;
    private const byte NeutralColorTolerance = 16;

    private readonly byte[] _imageBytes;
    private readonly int _pixelDataOffset;
    private IReadOnlyList<ScreenRectangle>? _buttons;

    private ScreenImage(byte[] imageBytes, int pixelDataOffset, int width, int height)
    {
        _imageBytes = imageBytes;
        _pixelDataOffset = pixelDataOffset;
        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }

    public static ScreenImage LoadPortablePixmap(byte[] imageBytes)
    {
        var headerOffset = 0;
        var magic = ReadPortablePixmapToken(imageBytes, ref headerOffset);

        if (magic != "P6")
            throw new InvalidOperationException($"Unsupported screen capture format '{magic}'");

        if (!int.TryParse(ReadPortablePixmapToken(imageBytes, ref headerOffset), NumberStyles.None, CultureInfo.InvariantCulture, out var width) || width <= 0)
            throw new InvalidOperationException("Screen capture has an invalid width");

        if (!int.TryParse(ReadPortablePixmapToken(imageBytes, ref headerOffset), NumberStyles.None, CultureInfo.InvariantCulture, out var height) || height <= 0)
            throw new InvalidOperationException("Screen capture has an invalid height");

        if (ReadPortablePixmapToken(imageBytes, ref headerOffset) != "255")
            throw new InvalidOperationException("Screen capture does not use 8-bit color channels");

        var pixelByteCount = checked(width * height * 3);
        var pixelDataOffset = imageBytes.Length - pixelByteCount;

        if (pixelDataOffset <= headerOffset)
            throw new InvalidOperationException("Screen capture pixel data is truncated");

        return new ScreenImage(imageBytes, pixelDataOffset, width, height);
    }

    public bool TryFindMainMenuMultiplayerButton(out ScreenRectangle multiplayerButton)
    {
        var expectedTop = (int)Math.Round(Height * 0.55);
        var candidates = FindButtons()
            .Where(button => button.Width >= Width * 0.4 && IsHorizontallyCentered(button))
            .Where(button => Math.Abs(button.Top - expectedTop) <= 5)
            .ToArray();

        if (candidates.Length > 0)
        {
            multiplayerButton = candidates.MinBy(button => Math.Abs(button.Top - expectedTop));
            return true;
        }

        multiplayerButton = default;
        return false;
    }

    public bool TryFindOnlinePlayWarningProceedButton(out ScreenRectangle proceedButton)
    {
        var warningButtonRows = GroupIntoRows(FindButtons().Where(button => button.Top >= Height * 0.55));
        var warningRow = warningButtonRows.SingleOrDefault(row => row.Count is 2 && row.All(button => button.Width >= Width * 0.25));

        if (warningRow is null)
        {
            proceedButton = default;
            return false;
        }

        proceedButton = warningRow.OrderBy(button => button.Left).First();
        return true;
    }

    public bool TryFindMultiplayerScreenDirectConnectionButton(out ScreenRectangle directConnectionButton)
    {
        var bottomButtons = FindButtons().Where(button => button.Top >= Height * 0.7).ToArray();

        if (bottomButtons.Length < 6)
        {
            directConnectionButton = default;
            return false;
        }

        var upperButtonRow = GroupIntoRows(bottomButtons)
            .Where(row => row.Count is 3)
            .OrderBy(row => row.Min(button => button.Top))
            .FirstOrDefault();

        if (upperButtonRow is null)
        {
            directConnectionButton = default;
            return false;
        }

        directConnectionButton = upperButtonRow.OrderBy(button => button.Left).ElementAt(1);
        return true;
    }

    public bool TryFindConnectionFailureBackButton(out ScreenRectangle backButton)
    {
        var centeredWideButtons = FindButtons()
            .Where(button => button.Width >= Width * 0.4 && IsHorizontallyCentered(button))
            .ToArray();

        if (centeredWideButtons.Length is 1
            && centeredWideButtons[0].Top >= Height * 0.45
            && centeredWideButtons[0].Top <= Height * 0.75)
        {
            backButton = centeredWideButtons[0];
            return true;
        }

        backButton = default;
        return false;
    }

    public bool TryFindPauseMenuBackToGameButton(out ScreenRectangle backToGameButton)
    {
        var buttons = FindButtons();
        var centeredWideButtons = buttons
            .Where(button => button.Width >= Width * 0.4 && IsHorizontallyCentered(button))
            .OrderBy(button => button.Top)
            .ToArray();

        if (buttons.Count >= 5
            && centeredWideButtons.Length >= 2
            && centeredWideButtons[0].Top >= Height * 0.2
            && centeredWideButtons[0].Top <= Height * 0.45
            && centeredWideButtons[^1].Top >= Height * 0.6)
        {
            backToGameButton = centeredWideButtons[0];
            return true;
        }

        backToGameButton = default;
        return false;
    }

    public bool TryFindDirectConnectionScreen(out DirectConnectionScreen directConnectionScreen)
    {
        var centeredButtons = FindButtons()
            .Where(button => button.Width >= Width * 0.4 && IsHorizontallyCentered(button))
            .Where(button => button.Top >= Height * 0.55)
            .OrderBy(button => button.Top)
            .ToArray();

        for (var index = 0; index < centeredButtons.Length - 1; index++)
        {
            var joinButton = centeredButtons[index];
            var cancelButton = centeredButtons[index + 1];

            if (!HaveMatchingWidths(joinButton, cancelButton) || !HaveStandardVerticalSpacing(joinButton, cancelButton))
                continue;

            if (!TryFindServerAddressField(joinButton, out var serverAddressField))
                continue;

            directConnectionScreen = new DirectConnectionScreen(serverAddressField, joinButton, cancelButton);
            return true;
        }

        directConnectionScreen = default;
        return false;
    }

    public double CalculateDifferenceRatio(ScreenImage other, ScreenRectangle area, byte channelDifferenceThreshold = 20)
    {
        if (Width != other.Width || Height != other.Height)
            throw new ArgumentException("Screens must have matching dimensions.", nameof(other));

        if (area.Left < 0 || area.Top < 0 || area.Right > Width || area.Bottom > Height)
            throw new ArgumentOutOfRangeException(nameof(area));

        var differentPixels = 0;
        var comparedPixels = area.Width * area.Height;

        for (var y = area.Top; y < area.Bottom; y++)
        {
            for (var x = area.Left; x < area.Right; x++)
            {
                var leftPixel = GetPixel(x, y);
                var rightPixel = other.GetPixel(x, y);

                if (Math.Abs(leftPixel.Red - rightPixel.Red) > channelDifferenceThreshold
                    || Math.Abs(leftPixel.Green - rightPixel.Green) > channelDifferenceThreshold
                    || Math.Abs(leftPixel.Blue - rightPixel.Blue) > channelDifferenceThreshold)
                {
                    differentPixels++;
                }
            }
        }

        return comparedPixels is 0 ? 0 : (double)differentPixels / comparedPixels;
    }

    public bool IsServerAddressFieldEmpty(ScreenRectangle serverAddressField)
    {
        const int fieldContentInset = 6;
        const int maximumHorizontalCursorWidth = 12;
        const int maximumHorizontalCursorHeight = 3;
        const int maximumVerticalCursorWidth = 3;

        var fieldContent = serverAddressField.Inset(fieldContentInset);
        var brightPixelCount = 0;
        var brightPixelLeft = fieldContent.Right;
        var brightPixelTop = fieldContent.Bottom;
        var brightPixelRight = fieldContent.Left;
        var brightPixelBottom = fieldContent.Top;

        for (var y = fieldContent.Top; y < fieldContent.Bottom; y++)
        {
            for (var x = fieldContent.Left; x < fieldContent.Right; x++)
            {
                if (!IsNeutralAndBright(GetPixel(x, y)))
                    continue;

                brightPixelCount++;
                brightPixelLeft = Math.Min(brightPixelLeft, x);
                brightPixelTop = Math.Min(brightPixelTop, y);
                brightPixelRight = Math.Max(brightPixelRight, x + 1);
                brightPixelBottom = Math.Max(brightPixelBottom, y + 1);
            }
        }

        if (brightPixelCount is 0)
            return true;

        var brightPixelWidth = brightPixelRight - brightPixelLeft;
        var brightPixelHeight = brightPixelBottom - brightPixelTop;
        var isHorizontalCursor = brightPixelWidth <= maximumHorizontalCursorWidth
            && brightPixelHeight <= maximumHorizontalCursorHeight
            && brightPixelTop >= fieldContent.Top + fieldContent.Height / 2;
        var isVerticalCursor = brightPixelWidth <= maximumVerticalCursorWidth
            && brightPixelHeight >= fieldContent.Height / 3;

        return isHorizontalCursor || isVerticalCursor;
    }

    public void Dispose()
    {
        // The image is stored in managed memory and needs no explicit cleanup.
    }

    public override string ToString()
    {
        return $"{Width}x{Height}; buttons: {string.Join(", ", FindButtons())}";
    }

    private IReadOnlyList<ScreenRectangle> FindButtons()
    {
        return _buttons ??= DetectButtons();
    }

    private IReadOnlyList<ScreenRectangle> DetectButtons()
    {
        var candidates = new List<ScreenRectangle>();

        for (var top = 0; top <= Height - MinimumButtonHeight; top++)
        {
            foreach (var (left, width) in FindBlackRuns(top))
            {
                if (width is < MinimumButtonWidth or > MaximumButtonWidth)
                    continue;

                for (var height = MinimumButtonHeight; height <= MaximumButtonHeight && top + height <= Height; height++)
                {
                    var candidate = new ScreenRectangle(left, top, width, height);

                    if (!HasButtonBorder(candidate) || !HasButtonInterior(candidate))
                        continue;

                    candidates.Add(candidate);
                    break;
                }
            }
        }

        var buttons = new List<ScreenRectangle>();

        foreach (var candidate in candidates.OrderBy(button => button.Top).ThenBy(button => button.Left))
        {
            var duplicateIndex = buttons.FindIndex(button => AreDuplicateDetections(button, candidate));

            if (duplicateIndex < 0)
            {
                buttons.Add(candidate);
                continue;
            }

            if (candidate.Width * candidate.Height > buttons[duplicateIndex].Width * buttons[duplicateIndex].Height)
                buttons[duplicateIndex] = candidate;
        }

        return buttons.OrderBy(button => button.Top).ThenBy(button => button.Left).ToArray();
    }

    private IEnumerable<(int Left, int Width)> FindBlackRuns(int y)
    {
        var left = 0;

        while (left < Width)
        {
            while (left < Width && !IsBlack(GetPixel(left, y)))
                left++;

            var right = left;

            while (right < Width && IsBlack(GetPixel(right, y)))
                right++;

            if (right > left)
                yield return (left, right - left);

            left = right + 1;
        }
    }

    private bool HasButtonBorder(ScreenRectangle candidate)
    {
        var topBlackPixels = 0;
        var bottomBlackPixels = 0;

        for (var x = candidate.Left; x < candidate.Right; x++)
        {
            if (IsBlack(GetPixel(x, candidate.Top)))
                topBlackPixels++;

            if (IsBlack(GetPixel(x, candidate.Bottom - 1)))
                bottomBlackPixels++;
        }

        if ((double)topBlackPixels / candidate.Width < 0.9 || (double)bottomBlackPixels / candidate.Width < 0.85)
            return false;

        var leftBlackPixels = 0;
        var rightBlackPixels = 0;

        for (var y = candidate.Top; y < candidate.Bottom; y++)
        {
            if (IsBlack(GetPixel(candidate.Left, y)))
                leftBlackPixels++;

            if (IsBlack(GetPixel(candidate.Right - 1, y)))
                rightBlackPixels++;
        }

        return (double)leftBlackPixels / candidate.Height >= 0.75 && (double)rightBlackPixels / candidate.Height >= 0.75;
    }

    private bool HasButtonInterior(ScreenRectangle candidate)
    {
        var inset = Math.Max(3, candidate.Height / 10);
        var sideSectionWidth = Math.Max(1, candidate.Width / 4 - inset);
        var neutralPixels = 0;
        var sufficientlyBrightPixels = 0;
        var sampleCount = 0;

        for (var y = candidate.Top + inset; y < candidate.Bottom - inset; y += 2)
        {
            foreach (var x in EnumerateSideSectionPixels(candidate, inset, sideSectionWidth))
            {
                var pixel = GetPixel(x, y);
                sampleCount++;

                if (IsNeutral(pixel))
                    neutralPixels++;

                if (GetBrightness(pixel) >= 24)
                    sufficientlyBrightPixels++;
            }
        }

        return sampleCount > 0
            && (double)neutralPixels / sampleCount >= 0.7
            && (double)sufficientlyBrightPixels / sampleCount >= 0.7;
    }

    private IEnumerable<int> EnumerateSideSectionPixels(ScreenRectangle candidate, int inset, int sideSectionWidth)
    {
        var leftStart = candidate.Left + inset;
        var rightStart = candidate.Right - inset - sideSectionWidth;

        for (var offset = 0; offset < sideSectionWidth; offset += 2)
        {
            yield return leftStart + offset;
            yield return rightStart + offset;
        }
    }

    private bool TryFindServerAddressField(ScreenRectangle joinButton, out ScreenRectangle serverAddressField)
    {
        var minimumFieldWidth = (int)(joinButton.Width * 0.95);
        var maximumFieldWidth = (int)(joinButton.Width * 1.05);

        for (var top = joinButton.Top - 1; top >= Height * 0.25; top--)
        {
            foreach (var (left, width) in FindNeutralBrightRuns(top))
            {
                if (width < minimumFieldWidth || width > maximumFieldWidth || Math.Abs(left + width / 2 - Width / 2) > 5)
                    continue;

                for (var height = MinimumButtonHeight; height <= 48 && top + height < joinButton.Top; height++)
                {
                    var candidate = new ScreenRectangle(left, top, width, height);

                    if (HasTextFieldInterior(candidate))
                    {
                        serverAddressField = candidate;
                        return true;
                    }
                }
            }
        }

        serverAddressField = default;
        return false;
    }

    private IEnumerable<(int Left, int Width)> FindNeutralBrightRuns(int y)
    {
        var left = 0;

        while (left < Width)
        {
            while (left < Width && !IsNeutralAndBright(GetPixel(left, y)))
                left++;

            var right = left;

            while (right < Width && IsNeutralAndBright(GetPixel(right, y)))
                right++;

            if (right > left)
                yield return (left, right - left);

            left = right + 1;
        }
    }

    private bool HasTextFieldInterior(ScreenRectangle candidate)
    {
        var bottomBorderPixels = 0;
        var darkInteriorPixels = 0;
        var interiorSampleCount = 0;

        for (var x = candidate.Left; x < candidate.Right; x++)
        {
            if (IsNeutralAndBright(GetPixel(x, candidate.Bottom - 1)))
                bottomBorderPixels++;
        }

        for (var y = candidate.Top + 3; y < candidate.Bottom - 3; y += 2)
        {
            for (var x = candidate.Left + 6; x < candidate.Right - 6; x += 4)
            {
                interiorSampleCount++;

                if (GetBrightness(GetPixel(x, y)) <= 20)
                    darkInteriorPixels++;
            }
        }

        return (double)bottomBorderPixels / candidate.Width >= 0.8
            && interiorSampleCount > 0
            && (double)darkInteriorPixels / interiorSampleCount >= 0.8;
    }

    private IReadOnlyList<IReadOnlyList<ScreenRectangle>> GroupIntoRows(IEnumerable<ScreenRectangle> buttons)
    {
        var rows = new List<List<ScreenRectangle>>();

        foreach (var button in buttons.OrderBy(button => button.Top))
        {
            var row = rows.FirstOrDefault(candidateRow => Math.Abs(candidateRow[0].Top - button.Top) <= 3);

            if (row is null)
            {
                row = [];
                rows.Add(row);
            }

            row.Add(button);
        }

        return rows.Select(row => (IReadOnlyList<ScreenRectangle>)row).ToArray();
    }

    private PixelColor GetPixel(int x, int y)
    {
        var offset = _pixelDataOffset + (y * Width + x) * 3;
        return new PixelColor(_imageBytes[offset], _imageBytes[offset + 1], _imageBytes[offset + 2]);
    }

    private bool IsHorizontallyCentered(ScreenRectangle rectangle)
    {
        return Math.Abs(rectangle.Left + rectangle.Width / 2 - Width / 2) <= 5;
    }

    private static string ReadPortablePixmapToken(byte[] imageBytes, ref int offset)
    {
        while (offset < imageBytes.Length)
        {
            while (offset < imageBytes.Length && char.IsWhiteSpace((char)imageBytes[offset]))
                offset++;

            if (offset >= imageBytes.Length || imageBytes[offset] is not (byte)'#')
                break;

            while (offset < imageBytes.Length && imageBytes[offset] is not (byte)'\n')
                offset++;
        }

        var tokenStart = offset;

        while (offset < imageBytes.Length && !char.IsWhiteSpace((char)imageBytes[offset]))
            offset++;

        if (tokenStart == offset)
            throw new InvalidOperationException("Screen capture has an incomplete portable pixmap header");

        return System.Text.Encoding.ASCII.GetString(imageBytes, tokenStart, offset - tokenStart);
    }

    private static bool HaveMatchingWidths(ScreenRectangle first, ScreenRectangle second)
    {
        return Math.Abs(first.Width - second.Width) <= 5;
    }

    private static bool HaveStandardVerticalSpacing(ScreenRectangle upper, ScreenRectangle lower)
    {
        var spacing = lower.Top - upper.Bottom;
        return spacing is >= 5 and <= 12;
    }

    private static bool AreDuplicateDetections(ScreenRectangle first, ScreenRectangle second)
    {
        return Math.Abs(first.Left - second.Left) <= 3
            && Math.Abs(first.Top - second.Top) <= 3
            && Math.Abs(first.Right - second.Right) <= 3
            && Math.Abs(first.Bottom - second.Bottom) <= 3;
    }

    private static bool IsBlack(PixelColor pixel)
    {
        return pixel.Red <= BlackThreshold && pixel.Green <= BlackThreshold && pixel.Blue <= BlackThreshold;
    }

    private static bool IsNeutral(PixelColor pixel)
    {
        var minimum = Math.Min(pixel.Red, Math.Min(pixel.Green, pixel.Blue));
        var maximum = Math.Max(pixel.Red, Math.Max(pixel.Green, pixel.Blue));
        return maximum - minimum <= NeutralColorTolerance;
    }

    private static bool IsNeutralAndBright(PixelColor pixel)
    {
        var brightness = GetBrightness(pixel);
        return IsNeutral(pixel) && brightness >= 80;
    }

    private static byte GetBrightness(PixelColor pixel)
    {
        return (byte)((pixel.Red + pixel.Green + pixel.Blue) / 3);
    }
}

readonly record struct PixelColor(byte Red, byte Green, byte Blue);

readonly record struct ScreenRectangle(int Left, int Top, int Width, int Height)
{
    public int Right => Left + Width;
    public int Bottom => Top + Height;
    public int CenterX => Left + Width / 2;
    public int CenterY => Top + Height / 2;

    public ScreenRectangle Inset(int amount)
    {
        if (amount < 0 || Width <= amount * 2 || Height <= amount * 2)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new ScreenRectangle(Left + amount, Top + amount, Width - amount * 2, Height - amount * 2);
    }

    public override string ToString()
    {
        return $"({Left},{Top}) {Width}x{Height}";
    }
}

readonly record struct DirectConnectionScreen(ScreenRectangle ServerAddressField, ScreenRectangle JoinButton, ScreenRectangle CancelButton);

readonly record struct NavigationScreenTarget(NavigationScreenKind Kind, ScreenRectangle Target);

enum NavigationScreenKind
{
    MultiplayerServerList,
    OnlinePlayWarning
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
