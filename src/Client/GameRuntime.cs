using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using File = System.IO.File;

namespace Void.Client;

/// <summary>
/// Owns the Linux, PortableMC, CurseForge, X11, and visual-automation details for one Minecraft game process.
/// Lifecycle coordination deliberately lives in <see cref="GameCoordinator"/> so this class has no shared-state locks.
/// </summary>
internal sealed partial class GameRuntime : IGameRuntime
{
    private const string DefaultMinecraftDirectory = "/root/.minecraft";
    private const string DefaultCurseForgeApiBaseUrl = "https://api.curseforge.com";
    private const string DefaultDisplay = ":99";
    private const string DisplayScreenWidth = "854";
    private const string DisplayScreenHeight = "480";
    private const string DisplayScreenResolution = $"{DisplayScreenWidth}x{DisplayScreenHeight}";
    private const string PortableMinecraftLegacyJvmExecutablePath = "/opt/zulu-8-i686/bin/java";
    private const string PortableMinecraftLegacyJvmPath = "/usr/local/bin/java-i686";
    private const string PortableMinecraftLauncherPath = "/usr/local/bin/launch-portableminecraftclient";
    private const string PortableMinecraftDryRunPath = "/usr/local/bin/portablemc-dry-run-with-retries";
    private const string PortableMinecraftOptionsPath = "/opt/portableminecraftclient/options.txt";
    private const string PortableMinecraftSodiumOptionsPath = "/opt/portableminecraftclient/sodium-options.json";
    private const string PortableMinecraftArmLwjgl3Version = "3.3.3";
    private const string PortableMinecraftArmLwjgl4Version = "3.4.1";
    private const string PortableMinecraftArmLwjgl4ClassPath = "/opt/portableminecraftclient/lwjgl-3.4.1-unsafe.jar";
    private const string PortableMinecraftArmLwjgl4NativePath = "/opt/portableminecraftclient/lwjgl-3.4.1-natives-linux-arm64.jar";
    private const string PortableMinecraftArmVulkanLibrary = "org.lwjgl:lwjgl-vulkan:*:natives-linux-arm64";
    private const string ChatInputBrightnessCropGeometry = "854x2+0+451";
    private const int MinecraftGameId = 432;
    private const int CurseForgeFilesBatchSize = 50;
    private const int UserInterfaceStableConfirmationCount = 2;
    private const int ButtonStableConfirmationMilliseconds = 1000;
    private const double ButtonHoverDifferenceRatioThreshold = 0.03;
    private const double ServerAddressFieldDifferenceRatioThreshold = 0.01;
    private const double ChatInputBrightnessRatioThreshold = 0.7;
    private const int DisplayProbeTimeoutMilliseconds = 1000;
    private const int ExternalProcessTimeoutMilliseconds = 5000;
    private const int ScreenCaptureTimeoutMilliseconds = 10000;
    private const int ProcessStopTimeoutMilliseconds = 10000;
    private const int CriticalProcessEarlyExitMilliseconds = 1000;

    private bool _chatInputTargetsWindow = true;
    private bool _chatInputSupportsVisualConfirmation = true;

    public async Task WriteOptionsAsync(string options, CancellationToken cancellationToken)
    {
        var minecraftDirectory = GetMinecraftDirectory();
        Directory.CreateDirectory(minecraftDirectory);
        var destinationPath = Path.Combine(minecraftDirectory, "options.txt");
        var temporaryPath = $"{destinationPath}.{Guid.NewGuid():N}.tmp";

        try
        {
            await File.WriteAllTextAsync(temporaryPath, options, cancellationToken);
            File.Move(temporaryPath, destinationPath, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    public async Task<RunningGame> LaunchPortableAsync(string portableMinecraftVersion, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        var minecraftDirectory = GetMinecraftDirectory();
        await PreparePortableMinecraftClientAsync(minecraftDirectory, portableMinecraftVersion, cancellationToken);
        return await LaunchPreparedGameAsync(minecraftDirectory, portableMinecraftVersion, arguments, cancellationToken);
    }

    public Task<RunningGame> LaunchVanillaAsync(string version, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        return LaunchPortableAsync($"mojang:{version}", arguments, cancellationToken);
    }

    public async Task<RunningGame> LaunchCurseForgeAsync(string slug, int fileId, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("CURSEFORGE_API_KEY is not set");

        var minecraftDirectory = GetMinecraftDirectory();
        var portableMinecraftVersion = await PrepareCurseForgeAsync(slug, fileId, apiKey, CreateCurseForgeApiBaseUri(), minecraftDirectory, cancellationToken);
        return await LaunchPreparedGameAsync(minecraftDirectory, portableMinecraftVersion, arguments, cancellationToken);
    }

    public Task ConnectAsync(string host, int port, CancellationToken cancellationToken)
    {
        return JoinServerOperationAsync(host, port, cancellationToken);
    }

    public Task SendChatAsync(string message, CancellationToken cancellationToken)
    {
        return SendChatOperationAsync(message, cancellationToken);
    }

    public Task<byte[]> CaptureScreenshotAsync(CancellationToken cancellationToken)
    {
        return CaptureScreenAsync(cancellationToken);
    }

    public async Task<StopMode> StopAsync(RunningGame? game, CancellationToken cancellationToken)
    {
        if (game is null || game.Process.HasExited)
            return StopMode.AlreadyStopped;

        var terminateResult = await RunProcessTextAsync(CreateProcessInfo("kill", ["-TERM", "--", $"-{game.Process.Id}"]), TimeSpan.FromSeconds(5), cancellationToken);

        if (terminateResult.ExitCode is not 0 && !game.Process.HasExited)
            throw new InvalidOperationException($"kill -TERM failed with code {terminateResult.ExitCode}: {terminateResult.StandardError}");

        try
        {
            await WaitForManagedProcessExitAsync(game.Process, TimeSpan.FromMilliseconds(ProcessStopTimeoutMilliseconds), cancellationToken);
            return StopMode.Graceful;
        }
        catch (TimeoutException)
        {
            game.Process.KillTree();
            await game.Process.WaitForExitAsync(CancellationToken.None);
            return StopMode.Forced;
        }
    }

    private async Task<RunningGame> LaunchPreparedGameAsync(string minecraftDirectory, string portableMinecraftVersion, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        await PrepareDisplayAndWindowAsync(cancellationToken);
        Console.Error.WriteLine($"Launching Minecraft with PortableMC version: {portableMinecraftVersion}");
        var process = LaunchPortableMinecraftClient(minecraftDirectory, portableMinecraftVersion, arguments.Cast<string?>().ToArray(), cancellationToken);

        try
        {
            await WaitForPreparedLargestWindowAsync(Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay, cancellationToken);
            return new RunningGame(new ManagedProcess(process), portableMinecraftVersion, DateTimeOffset.UtcNow);
        }
        catch
        {
            KillProcess(process);
            await WaitForKilledProcessAsync(process);
            throw;
        }
    }

    private static string GetMinecraftDirectory()
    {
        return Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? DefaultMinecraftDirectory;
    }

    private static async Task WaitForManagedProcessExitAsync(IManagedProcess process, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var timeoutSource = new CancellationTokenSource(timeout);
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);

        try
        {
            await process.WaitForExitAsync(linkedSource.Token);
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"Minecraft did not exit within {timeout.TotalSeconds:F1} seconds");
        }
    }

    async Task PreparePortableMinecraftClientAsync(string minecraftDirectory, string portableMinecraftVersion, CancellationToken cancellationToken)
    {
        var configDirectory = Path.Combine(minecraftDirectory, "config");
        var modsDirectory = Path.Combine(minecraftDirectory, "mods");
        var optionsPath = Path.Combine(minecraftDirectory, "options.txt");
        var sodiumOptionsPath = Path.Combine(configDirectory, "sodium-options.json");
        var serversPath = Path.Combine(minecraftDirectory, "servers.dat");
        Directory.CreateDirectory(configDirectory);
        Directory.CreateDirectory(modsDirectory);

        if (!File.Exists(optionsPath) || new FileInfo(optionsPath).Length == 0)
            File.Copy(PortableMinecraftOptionsPath, optionsPath, true);

        if (!File.Exists(sodiumOptionsPath))
            File.Copy(PortableMinecraftSodiumOptionsPath, sodiumOptionsPath);

        if (!File.Exists(serversPath))
            await File.WriteAllBytesAsync(serversPath, Convert.FromHexString("0a0000090007736572766572730a0000000101000668696464656e000800026970000a766f69643a32353536350800046e616d65000a566f69642050726f78790000"), cancellationToken);

        var portableMinecraftArguments = new List<string> { portableMinecraftVersion, "--demo", "--main-dir", minecraftDirectory, "--output", "machine" };

        if (File.Exists(PortableMinecraftLegacyJvmExecutablePath) && !portableMinecraftVersion.StartsWith("mojang:", StringComparison.Ordinal))
        {
            portableMinecraftArguments.AddRange(["--fix-lwjgl", PortableMinecraftArmLwjgl4Version]);
            portableMinecraftArguments.AddRange(["--exclude-lib", PortableMinecraftArmVulkanLibrary]);
            portableMinecraftArguments.AddRange(["--include-class", PortableMinecraftArmLwjgl4ClassPath]);
            portableMinecraftArguments.AddRange(["--include-class", PortableMinecraftArmLwjgl4NativePath]);
        }

        var preparationResult = await RunProcessTextAsync(CreateProcessInfo(PortableMinecraftDryRunPath, portableMinecraftArguments), TimeSpan.FromMinutes(5), cancellationToken);

        if (!string.IsNullOrWhiteSpace(preparationResult.StandardError))
            Console.Error.Write(preparationResult.StandardError);

        if (preparationResult.ExitCode != 0)
            throw new InvalidOperationException($"Portable Minecraft preparation exited with code {preparationResult.ExitCode}: {preparationResult.StandardError}");

        var minecraftVersion = preparationResult.StandardOutput
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r').Split('\t'))
            .Where(fields => fields.Length > 1 && fields[0] == "loaded_hierarchy")
            .Select(fields => fields[^1])
            .LastOrDefault();

        if (string.IsNullOrWhiteSpace(minecraftVersion))
            throw new InvalidOperationException("Portable Minecraft preparation did not report a Minecraft version");

        await InstallSodiumAsync(modsDirectory, portableMinecraftVersion, minecraftVersion, cancellationToken);
    }

    async Task InstallSodiumAsync(string modsDirectory, string portableMinecraftVersion, string minecraftVersion, CancellationToken cancellationToken)
    {
        var sodiumPath = Path.Combine(modsDirectory, "sodium.jar");
        var temporarySodiumPath = Path.Combine(modsDirectory, $".sodium.jar.{Guid.NewGuid():N}");
        var separatorIndex = portableMinecraftVersion.IndexOf(':');
        var loader = separatorIndex < 0 ? "mojang" : portableMinecraftVersion[..separatorIndex];
        var loaders = Uri.EscapeDataString(JsonSerializer.Serialize(new[] { loader }));
        var gameVersions = Uri.EscapeDataString(JsonSerializer.Serialize(new[] { minecraftVersion }));
        string? sodiumUrl = null;

        File.Delete(sodiumPath);

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("caunt/Void");
            using var versionsResponse = await httpClient.GetAsync($"https://api.modrinth.com/v2/project/AANobbMI/version?loaders={loaders}&game_versions={gameVersions}", cancellationToken);
            versionsResponse.EnsureSuccessStatusCode();
            await using var versionsStream = await versionsResponse.Content.ReadAsStreamAsync(cancellationToken);
            using var versionsDocument = await JsonDocument.ParseAsync(versionsStream, cancellationToken: cancellationToken);

            foreach (var version in versionsDocument.RootElement.EnumerateArray())
            {
                if (!version.TryGetProperty("files", out var files))
                    continue;

                foreach (var file in files.EnumerateArray())
                {
                    if (file.TryGetProperty("primary", out var primary) && primary.GetBoolean() && file.TryGetProperty("url", out var url))
                    {
                        sodiumUrl = url.GetString();
                        break;
                    }
                }

                if (sodiumUrl is not null)
                    break;
            }

            if (sodiumUrl is null)
            {
                Console.Error.WriteLine($"Sodium was not found for {loader} Minecraft {minecraftVersion}");
                return;
            }

            using var sodiumResponse = await httpClient.GetAsync(sodiumUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            sodiumResponse.EnsureSuccessStatusCode();
            await using (var sodiumFile = File.Create(temporarySodiumPath))
                await sodiumResponse.Content.CopyToAsync(sodiumFile, cancellationToken);

            File.Move(temporarySodiumPath, sodiumPath, true);
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException)
        {
            Console.Error.WriteLine($"Sodium download failed for {loader} Minecraft {minecraftVersion}: {exception.Message}");
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            Console.Error.WriteLine($"Sodium download failed for {loader} Minecraft {minecraftVersion}: {exception.Message}");
        }
        finally
        {
            File.Delete(temporarySodiumPath);
        }
    }

    async Task<string> PrepareCurseForgeAsync(string slug, int fileId, string curseForgeApiKey, Uri curseForgeApiBaseUri, string minecraftDirectory, CancellationToken cancellationToken)
    {
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

        return portablemcVersion;
    }

    async Task SendChatOperationAsync(string message, CancellationToken cancellationToken)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
        var windowId = await FindLargestWindow(display, cancellationToken);

        if (windowId is null)
            throw new InvalidOperationException("no visible window found");

        var preferredInputWindowId = _chatInputTargetsWindow ? windowId : null;
        await ResizeWindowToDisplayAsync(windowId, cancellationToken);
        await RunOrThrow(cancellationToken, "xdotool", "windowfocus", "--sync", windowId);
        windowId = await ResumeGameIfPausedAsync(windowId, display, cancellationToken);
        var inputWindowId = await OpenChatAsync(windowId, preferredInputWindowId, display, cancellationToken);
        await TypeTextAsync(inputWindowId, message, _chatInputSupportsVisualConfirmation ? 50 : 150, cancellationToken);
        await SubmitChatAsync(windowId, inputWindowId, display, cancellationToken);
    }

    async Task JoinServerOperationAsync(string host, int port, CancellationToken cancellationToken)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
        var windowId = await WaitForPreparedLargestWindowAsync(display, cancellationToken);
        var serverAddress = $"{host}:{port}";

        windowId = await VisuallyClickButtonAsync("Multiplayer", windowId, display, screen => screen.TryFindMainMenuMultiplayerButton(out var button) ? button : null, cancellationToken);

        var nextScreen = await WaitForMultiplayerOrOnlinePlayWarningAsync(windowId, display, cancellationToken);

        if (nextScreen.Kind is NavigationScreenKind.OnlinePlayWarning)
        {
            Console.Error.WriteLine("Visually confirmed the third-party online play warning");
            windowId = await VisuallyClickButtonAsync("Proceed", windowId, display, screen => screen.TryFindOnlinePlayWarningProceedButton(out var button) ? button : null, cancellationToken);
            await WaitForScreenTargetAsync("multiplayer server list", windowId, display, screen => screen.TryFindMultiplayerScreenDirectConnectionButton(out var button) ? button : null, cancellationToken);
        }

        while (true)
        {
            windowId = await SubmitDirectConnectionAsync(windowId, display, serverAddress, cancellationToken);

            if (await WaitForInteractiveGameScreenAsync(windowId, display, cancellationToken))
                break;

            Console.Error.WriteLine("Visually confirmed that the connection failed; returning to the server list to retry");
            windowId = await VisuallyClickButtonAsync("Back to server list", windowId, display, screen => screen.TryFindConnectionFailureBackButton(out var button) ? button : null, cancellationToken);
            await WaitForScreenTargetAsync("multiplayer server list", windowId, display, screen => screen.TryFindMultiplayerScreenDirectConnectionButton(out var button) ? button : null, cancellationToken);
        }

        Console.Error.WriteLine($"Visually confirmed navigation from the main menu to server {serverAddress}");
    }

    async Task<string> SubmitDirectConnectionAsync(string windowId, string display, string serverAddress, CancellationToken cancellationToken)
    {
        windowId = await VisuallyClickButtonAsync("Direct Connection", windowId, display, screen => screen.TryFindMultiplayerScreenDirectConnectionButton(out var button) ? button : null, cancellationToken);
        await WaitForScreenTargetAsync("direct connection form", windowId, display, screen => screen.TryFindDirectConnectionScreen(out var directConnectionScreen) ? directConnectionScreen.JoinButton : null, cancellationToken);
        await EnterServerAddressAsync(windowId, display, serverAddress, cancellationToken);
        windowId = await VisuallyClickButtonAsync("Join Server", windowId, display, screen => screen.TryFindDirectConnectionScreen(out var directConnectionScreen) ? directConnectionScreen.JoinButton : null, cancellationToken);
        await WaitForDirectConnectionScreenToCloseAsync(windowId, display, cancellationToken);
        return windowId;
    }

    async Task<bool> WaitForInteractiveGameScreenAsync(string windowId, string display, CancellationToken cancellationToken)
    {
        var preferredInputWindowId = _chatInputTargetsWindow ? windowId : null;
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

            if (_chatInputSupportsVisualConfirmation)
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
                await ClearChatInputAsync(pressKeysSlowly: !_chatInputSupportsVisualConfirmation, cancellationToken: cancellationToken);

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

                if (failureConfirmationCount >= UserInterfaceStableConfirmationCount)
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

        windowId = await ResumeGameIfPausedAsync(windowId, display, cancellationToken);

        Console.Error.WriteLine("Visually confirmed an interactive in-game screen");
        return true;
    }

    async Task<string> ResumeGameIfPausedAsync(string windowId, string display, CancellationToken cancellationToken)
    {
        using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

        if (!screen.TryFindPauseMenuBackToGameButton(out _))
            return windowId;

        Console.Error.WriteLine("Visually confirmed the pause menu; returning to the game");
        windowId = await VisuallyClickButtonAsync("Back to Game", windowId, display, currentScreen => currentScreen.TryFindPauseMenuBackToGameButton(out var button) ? button : null, cancellationToken);

        while (true)
        {
            using var currentScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);

            if (!currentScreen.TryFindPauseMenuBackToGameButton(out _))
                return windowId;

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

    async Task<string> WaitForPreparedLargestWindowAsync(string display, CancellationToken cancellationToken)
    {
        while (true)
        {
            var windowId = await WaitForLargestWindowAsync(display, cancellationToken);

            try
            {
                await ResizeWindowToDisplayAsync(windowId, cancellationToken);
                await RunOrThrow(cancellationToken, "xdotool", "windowfocus", windowId);
                return windowId;
            }
            catch (InvalidOperationException)
            {
                // Minecraft can replace its startup window between discovery and focus. Try the new window.
            }
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

                if (target is not null && stableConfirmationCount >= UserInterfaceStableConfirmationCount)
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

                if (target is not null && stableConfirmationCount >= UserInterfaceStableConfirmationCount)
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

    async Task<string> VisuallyClickButtonAsync(string buttonName, string windowId, string display, Func<ScreenImage, ScreenRectangle?> locateButton, CancellationToken cancellationToken)
    {
        ScreenRectangle? previousButton = null;
        var stableButtonStopwatch = Stopwatch.StartNew();

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await MoveMouseAsync(windowId, 2, 2, cancellationToken);
                using var baselineScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
                var button = locateButton(baselineScreen);

                if (button is null)
                {
                    previousButton = null;
                    stableButtonStopwatch.Restart();
                    continue;
                }

                if (previousButton is null || !AreMatchingRectangles(previousButton.Value, button.Value))
                {
                    previousButton = button;
                    stableButtonStopwatch.Restart();
                    continue;
                }

                if (stableButtonStopwatch.ElapsedMilliseconds < ButtonStableConfirmationMilliseconds)
                    continue;

                using var confirmedBaselineScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
                var confirmedButton = locateButton(confirmedBaselineScreen);

                if (confirmedButton is null || !AreMatchingRectangles(button.Value, confirmedButton.Value))
                {
                    previousButton = confirmedButton;
                    stableButtonStopwatch.Restart();
                    continue;
                }

                button = confirmedButton;
                await MoveMouseAsync(windowId, button.Value.CenterX, button.Value.CenterY, cancellationToken);
                var hoverConfirmationCount = 0;

                while (hoverConfirmationCount < UserInterfaceStableConfirmationCount)
                {
                    using var hoveredScreen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
                    var differenceRatio = confirmedBaselineScreen.CalculateDifferenceRatio(hoveredScreen, button.Value);

                    if (differenceRatio < ButtonHoverDifferenceRatioThreshold)
                    {
                        hoverConfirmationCount = 0;
                        continue;
                    }

                    hoverConfirmationCount++;

                    if (hoverConfirmationCount >= UserInterfaceStableConfirmationCount)
                        Console.Error.WriteLine($"Visually confirmed {buttonName} hover at {button.Value} ({differenceRatio:P1} changed pixels)");
                }

                await ClickMouseAsync(cancellationToken);
                return windowId;
            }
            catch (InvalidOperationException)
            {
                // The game can replace its window or screen while it is still loading. Reacquire the target.
                windowId = await WaitForPreparedLargestWindowAsync(display, cancellationToken);
                previousButton = null;
                stableButtonStopwatch.Restart();
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

            if (fieldDifferenceRatio < ServerAddressFieldDifferenceRatioThreshold)
            {
                enteredConfirmationCount = 0;
                continue;
            }

            enteredConfirmationCount++;

            if (enteredConfirmationCount >= UserInterfaceStableConfirmationCount)
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

                    if (emptyConfirmationCount >= UserInterfaceStableConfirmationCount)
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

        while (closedConfirmationCount < UserInterfaceStableConfirmationCount)
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
        var captureResult = await RunProcessBytesAsync(captureProcessInfo, TimeSpan.FromMilliseconds(ScreenCaptureTimeoutMilliseconds), cancellationToken);

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
        _chatInputTargetsWindow = !UsesActiveWindowChatInput(version);
        _chatInputSupportsVisualConfirmation = !RequiresBlindChatInput(version);

        var process = StartGameProcess(processInfo =>
        {
            processInfo.ArgumentList.Add("--main-dir");
            processInfo.ArgumentList.Add(directory);
            processInfo.ArgumentList.Add("start");
            processInfo.ArgumentList.Add(version);

            if (!HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--resolution"))
            {
                processInfo.ArgumentList.Add("--resolution");
                processInfo.ArgumentList.Add(DisplayScreenResolution);
            }

            if (File.Exists(PortableMinecraftLegacyJvmExecutablePath))
            {
                var isMojangVersion = version.StartsWith("mojang:", StringComparison.Ordinal);
                var usesLegacyLwjgl = isMojangVersion && UsesLegacyLwjgl(version);
                var armLwjglVersion = isMojangVersion ? GetArmLwjglVersion(version) : PortableMinecraftArmLwjgl4Version;

                if (usesLegacyLwjgl && !HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--jvm"))
                {
                    processInfo.ArgumentList.Add("--jvm");
                    processInfo.ArgumentList.Add(PortableMinecraftLegacyJvmPath);
                }
                else if (!usesLegacyLwjgl && !HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--fix-lwjgl"))
                {
                    processInfo.ArgumentList.Add("--fix-lwjgl");
                    processInfo.ArgumentList.Add(armLwjglVersion);

                    if (armLwjglVersion == PortableMinecraftArmLwjgl4Version)
                    {
                        processInfo.ArgumentList.Add("--include-class");
                        processInfo.ArgumentList.Add(PortableMinecraftArmLwjgl4ClassPath);
                        processInfo.ArgumentList.Add("--include-class");
                        processInfo.ArgumentList.Add(PortableMinecraftArmLwjgl4NativePath);
                    }
                }

                if (armLwjglVersion == PortableMinecraftArmLwjgl4Version && !HasPortableMinecraftArgument(requestedPortableMinecraftArguments, "--exclude-lib"))
                {
                    processInfo.ArgumentList.Add("--exclude-lib");
                    processInfo.ArgumentList.Add(PortableMinecraftArmVulkanLibrary);
                }
            }

            foreach (var argument in requestedPortableMinecraftArguments)
                processInfo.ArgumentList.Add(argument);
        });

        if (process.WaitForExit(CriticalProcessEarlyExitMilliseconds))
            throw new InvalidOperationException($"PortableMC exited immediately with code {process.ExitCode}");

        return process;
    }

    string GetArmLwjglVersion(string version)
    {
        var versionComponents = version["mojang:".Length..].Split('.');

        if (versionComponents.Length >= 1 && int.TryParse(versionComponents[0], out var majorVersion) && majorVersion >= 26)
            return PortableMinecraftArmLwjgl4Version;

        return PortableMinecraftArmLwjgl3Version;
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
            ? DefaultCurseForgeApiBaseUrl
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

        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
        var deadline = DateTimeOffset.UtcNow.AddSeconds(10);

        while (true)
        {
            if (await FindLargestWindow(display, cancellationToken) is null)
                return;

            if (DateTimeOffset.UtcNow >= deadline)
                throw new TimeoutException("A previous Minecraft window did not close before the next launch");

            await Task.Delay(100, cancellationToken);
        }
    }

    async Task<byte[]> CaptureScreenAsync(CancellationToken cancellationToken)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
        var windowId = await FindLargestWindow(display, cancellationToken);

        if (windowId is null)
            throw new InvalidOperationException("no visible window found");

        await ResizeWindowToDisplayAsync(windowId, cancellationToken);

        var captureProcessInfo = CreateProcessInfo("import", ["-window", windowId, "png:-"], display: display);
        var captureResult = await RunProcessBytesAsync(captureProcessInfo, TimeSpan.FromMilliseconds(ScreenCaptureTimeoutMilliseconds), cancellationToken);

        return captureResult.ExitCode is not 0
            ? throw new InvalidOperationException($"screen capture failed: {captureResult.StandardError}")
            : captureResult.StandardOutput;
    }

    async Task ResizeWindowToDisplayAsync(string windowId, CancellationToken cancellationToken = default)
    {
        await RunOrThrow(cancellationToken, "xdotool", "windowmove", "--sync", windowId, "0", "0");
        await RunOrThrow(cancellationToken, "xdotool", "windowsize", "--sync", windowId, DisplayScreenWidth, DisplayScreenHeight);
    }

    async Task EnsureDisplay(CancellationToken cancellationToken = default)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
        Environment.SetEnvironmentVariable("DISPLAY", display);
        await WaitForDisplayReadyAsync(display, cancellationToken);
    }

    async Task WaitForDisplayReadyAsync(string display, CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(10);

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (await IsDisplayReadyAsync(display, cancellationToken))
                return;

            await Task.Delay(100, cancellationToken);
        }

        throw new TimeoutException($"Display {display} did not become ready within 10 seconds");
    }

    async Task<bool> IsDisplayReadyAsync(string display, CancellationToken cancellationToken)
    {
        try
        {
            var processInfo = CreateProcessInfo("xdpyinfo", ["-display", display], display: display);
            var result = await RunProcessTextAsync(processInfo, TimeSpan.FromMilliseconds(DisplayProbeTimeoutMilliseconds), cancellationToken);

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
        if (!_chatInputSupportsVisualConfirmation)
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
        if (!_chatInputSupportsVisualConfirmation)
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
            var importResult = await RunProcessTextAsync(importProcessInfo, TimeSpan.FromMilliseconds(ScreenCaptureTimeoutMilliseconds), cancellationToken);

            if (importResult.ExitCode != 0)
                throw new InvalidOperationException($"chat input capture failed: {importResult.StandardError}");

            var convertProcessInfo = CreateProcessInfo("convert",
            [
                screenshotPath,
            "-colorspace",
            "Gray",
            "-crop",
            ChatInputBrightnessCropGeometry,
            "+repage",
            "-scale",
            "1x2!",
            "-format",
            "%[fx:u.p{0,0}.r*100] %[fx:u.p{0,1}.r*100]",
            "info:"
            ]);
            var convertResult = await RunProcessTextAsync(convertProcessInfo, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);

            if (convertResult.ExitCode != 0)
                throw new InvalidOperationException($"chat input analysis failed: {convertResult.StandardError}");

            var brightnessValues = convertResult.StandardOutput.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (brightnessValues.Length != 2
                || !double.TryParse(brightnessValues[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var brightnessAboveInput)
                || !double.TryParse(brightnessValues[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var inputBrightness))
            {
                throw new InvalidOperationException($"failed to parse chat input brightness values: '{convertResult.StandardOutput.Trim()}'");
            }

            return brightnessAboveInput >= 1 && inputBrightness / brightnessAboveInput <= ChatInputBrightnessRatioThreshold;
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
        var searchResult = await RunProcessTextAsync(searchProcessInfo, TimeSpan.FromMilliseconds(DisplayProbeTimeoutMilliseconds), cancellationToken);

        if (searchResult.ExitCode != 0)
            return null;

        string? largestWindowId = null;
        long largestArea = 0;

        foreach (var candidateWindowId in searchResult.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedCandidateId = candidateWindowId.Trim();
            var nameProcessInfo = CreateProcessInfo("xdotool", ["getwindowname", trimmedCandidateId], display: display);
            var nameResult = await RunProcessTextAsync(nameProcessInfo, TimeSpan.FromMilliseconds(DisplayProbeTimeoutMilliseconds), cancellationToken);

            if (nameResult.ExitCode != 0 || string.IsNullOrWhiteSpace(nameResult.StandardOutput))
                continue;

            var geometryProcessInfo = CreateProcessInfo("xdotool", ["getwindowgeometry", "--shell", trimmedCandidateId], display: display);
            var geometryResult = await RunProcessTextAsync(geometryProcessInfo, TimeSpan.FromMilliseconds(DisplayProbeTimeoutMilliseconds), cancellationToken);

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
        var result = await RunProcessTextAsync(processInfo, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);

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

    async Task WaitForProcessExitAsync(Process process, string processName, TimeSpan timeout, CancellationToken cancellationToken, bool killOnTimeout = true)
    {
        using var timeoutSource = new CancellationTokenSource(timeout);
        using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

        try
        {
            await process.WaitForExitAsync(linkedCancellationTokenSource.Token);
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            if (killOnTimeout)
            {
                KillProcess(process);
                await WaitForKilledProcessAsync(process);
            }

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

    Process StartGameProcess(Action<ProcessStartInfo> configure)
    {
        var processInfo = new ProcessStartInfo("setsid")
        {
            UseShellExecute = false
        };
        processInfo.ArgumentList.Add(PortableMinecraftLauncherPath);
        configure(processInfo);

        return Process.Start(processInfo)
               ?? throw new InvalidOperationException("Failed to start the PortableMC process group");
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
        var searchResult = await curseForgeClient.SearchModsAsync(MinecraftGameId, slug, cancellationToken);
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

                var resolvedFileId = file.FileId;

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
                    cancellationToken.ThrowIfCancellationRequested();

                    var batch = requiredFileIds.Skip(batchStart).Take(CurseForgeFilesBatchSize).ToList();
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
                    throw new InvalidOperationException($"Unsupported mod loader for CurseForge modpack '{slug}' (file id: {fileId}): {loaderId}");
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



    record ProcessTextResult(int ExitCode, string StandardOutput, string StandardError);

    record ProcessBytesResult(int ExitCode, byte[] StandardOutput, string StandardError);
}
