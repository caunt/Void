using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using File = System.IO.File;

namespace Void.Client;

/// <summary>
/// Owns the Linux, PortableMC, CurseForge, X11, and visual-automation details for one Minecraft game process.
/// Lifecycle coordination deliberately lives in <see cref="GameCoordinator"/> so this class has no shared-state locks.
/// </summary>
internal sealed partial class GameRuntime : IGameRuntime, IAsyncDisposable
{
    private const string DefaultMinecraftDirectory = "/root/.minecraft";
    private const string DefaultCurseForgeApiBaseUrl = "https://api.curseforge.com";
    private const string DefaultDisplay = ":99";
    private const string DisplayScreenWidth = "854";
    private const string DisplayScreenHeight = "480";
    private const string DisplayScreenResolution = $"{DisplayScreenWidth}x{DisplayScreenHeight}";
    private const string PortableMinecraftLegacyJvmExecutablePath = "/opt/zulu-8-x86_64/bin/java";
    private const string PortableMinecraftLegacyJvmPath = "/usr/local/bin/java-x86_64";
    private const string PortableMinecraftLauncherPath = "/usr/local/bin/launch-portableminecraftclient";
    private const string PortableMinecraftDryRunPath = "/usr/local/bin/portablemc-dry-run-with-retries";
    private const string PortableMinecraftJvmAttachPath = "/usr/bin/jattach";
    private const string PortableMinecraftOptionsPath = "/opt/portableminecraftclient/options.txt";
    private const string PortableMinecraftSodiumOptionsPath = "/opt/portableminecraftclient/sodium-options.json";
    private const string PortableMinecraftAgentPath = "/opt/portableminecraftclient/void-client-agent.jar";
    private const string PortableMinecraftArmLwjgl3Version = "3.3.3";
    private const string PortableMinecraftArmLwjgl4Version = "3.4.1";
    private const string PortableMinecraftArmLwjgl4ClassPath = "/opt/portableminecraftclient/lwjgl-3.4.1-unsafe.jar";
    private const string PortableMinecraftArmLwjgl4NativePath = "/opt/portableminecraftclient/lwjgl-3.4.1-natives-linux-arm64.jar";
    private const string PortableMinecraftArmVulkanLibrary = "org.lwjgl:lwjgl-vulkan:*:natives-linux-arm64";
    private const string ChatInputBrightnessCropGeometry = "854x2+0+451";
    private const int MinecraftGameId = 432;
    private const int CurseForgeFilesBatchSize = 50;
    private const int UserInterfacePollDelayMilliseconds = 100;
    private const int DisplayProbeTimeoutMilliseconds = 1000;
    private const int ExternalProcessTimeoutMilliseconds = 5000;
    private const int ScreenCaptureTimeoutMilliseconds = 10000;
    private const int ScreenCaptureMaximumAttempts = 3;
    private const int ProcessStopTimeoutMilliseconds = 10000;
    private const int CriticalProcessEarlyExitMilliseconds = 1000;
    private const int PlayerReadTimeoutMilliseconds = 2000;

    private bool _chatInputTargetsWindow = true;
    private bool _chatInputSupportsVisualConfirmation = true;
    private readonly GameTextRecognizer _textRecognizer = new();
    private long _nextWindowGeneration;

    public async ValueTask DisposeAsync()
    {
        await _textRecognizer.DisposeAsync();
        GC.SuppressFinalize(this);
    }

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

    private async Task<RunningGame> LaunchPortableAsync(string portableMinecraftVersion, IReadOnlyList<string> arguments, int? memoryMb, CancellationToken cancellationToken)
    {
        var minecraftDirectory = GetMinecraftDirectory();
        await PreparePortableMinecraftClientAsync(minecraftDirectory, portableMinecraftVersion, cancellationToken);
        return await LaunchPreparedGameAsync(minecraftDirectory, portableMinecraftVersion, arguments, memoryMb, cancellationToken);
    }

    public Task<RunningGame> LaunchVanillaAsync(string version, IReadOnlyList<string> arguments, int? memoryMb, CancellationToken cancellationToken)
    {
        return LaunchPortableAsync($"mojang:{version}", arguments, memoryMb, cancellationToken);
    }

    public Task<RunningGame> LaunchNeoForgeAsync(string version, IReadOnlyList<string> arguments, int? memoryMb, CancellationToken cancellationToken)
    {
        return LaunchPortableAsync($"neoforge:{version}", arguments, memoryMb, cancellationToken);
    }

    public async Task<RunningGame> LaunchCurseForgeAsync(string slug, int fileId, IReadOnlyList<string> arguments, int? memoryMb, CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("CURSEFORGE_API_KEY is not set");

        var minecraftDirectory = GetMinecraftDirectory();
        var portableMinecraftVersion = await PrepareCurseForgeAsync(slug, fileId, apiKey, CreateCurseForgeApiBaseUri(), minecraftDirectory, cancellationToken);
        return await LaunchPreparedGameAsync(minecraftDirectory, portableMinecraftVersion, arguments, memoryMb, cancellationToken);
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

    public async Task<GamePlayers> ReadPlayersAsync(RunningGame game, CancellationToken cancellationToken)
    {
        if (!File.Exists(game.Tracker.DescriptorPath))
            await AttachPlayerTrackerAsync(game, cancellationToken);

        using var timeoutSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(PlayerReadTimeoutMilliseconds));
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

        try
        {
            if (!File.Exists(game.Tracker.DescriptorPath))
                throw PlayersUnavailable("The Minecraft player tracker is not ready");

            var descriptor = await File.ReadAllTextAsync(game.Tracker.DescriptorPath, linkedSource.Token);

            if (!int.TryParse(descriptor, NumberStyles.None, CultureInfo.InvariantCulture, out var port) || port is < 1 or > 65535)
                throw PlayersUnavailable("The Minecraft player tracker published an invalid endpoint");

            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port, linkedSource.Token);
            await using var stream = client.GetStream();
            await using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            await writer.WriteLineAsync(game.Tracker.Token.AsMemory(), linkedSource.Token);
            var responseJson = await reader.ReadLineAsync(linkedSource.Token);

            if (string.IsNullOrWhiteSpace(responseJson))
                throw PlayersUnavailable("The Minecraft player tracker returned an empty response");

            var response = JsonSerializer.Deserialize<TrackerResponse>(responseJson, new JsonSerializerOptions(JsonSerializerDefaults.Web))
                           ?? throw PlayersUnavailable("The Minecraft player tracker returned a malformed response");

            if (response.Status is "notInWorld")
                throw new GamePlayersException(StatusCodes.Status409Conflict, response.Message ?? "The Minecraft client has no current player world");

            if (response.Status is not "ok")
                throw PlayersUnavailable(response.Message ?? "The Minecraft player tracker is unavailable");

            if (response.Local is null || response.Remote is null)
                throw PlayersUnavailable("The Minecraft player tracker returned an incomplete response");

            ValidatePlayer(response.Local);

            foreach (var player in response.Remote)
                ValidatePlayer(player);

            return new(response.Local, response.Remote);
        }
        catch (GamePlayersException)
        {
            throw;
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw PlayersUnavailable("The Minecraft player tracker did not respond in time", "response.timeout");
        }
        catch (Exception exception) when (exception is IOException or SocketException or JsonException or UnauthorizedAccessException)
        {
            throw PlayersUnavailable($"The Minecraft player tracker could not be read: {exception.Message}", "response.read", exception);
        }
    }

    public async Task<StopMode> StopAsync(RunningGame? game, CancellationToken cancellationToken)
    {
        if (game is null)
            return StopMode.AlreadyStopped;

        try
        {
            var launcherHasExited = game.Process.HasExited;
            var terminateResult = await RunProcessTextAsync(CreateProcessInfo("kill", ["-TERM", "--", $"-{game.Process.Id}"]), TimeSpan.FromSeconds(5), cancellationToken);

            if (terminateResult.ExitCode is not 0)
            {
                if ((launcherHasExited || game.Process.HasExited) && !IsProcessGroupRunning(game.Process.Id))
                    return StopMode.AlreadyStopped;

                throw new InvalidOperationException($"kill -TERM failed with code {terminateResult.ExitCode}: {terminateResult.StandardError}");
            }

            try
            {
                await WaitForProcessGroupExitAsync(game.Process.Id, TimeSpan.FromMilliseconds(ProcessStopTimeoutMilliseconds), cancellationToken);
                return StopMode.Graceful;
            }
            catch (TimeoutException)
            {
                var killResult = await RunProcessTextAsync(CreateProcessInfo("kill", ["-KILL", "--", $"-{game.Process.Id}"]), TimeSpan.FromSeconds(5), CancellationToken.None);

                if (killResult.ExitCode is not 0 && IsProcessGroupRunning(game.Process.Id))
                    throw new InvalidOperationException($"kill -KILL failed with code {killResult.ExitCode}: {killResult.StandardError}");

                await WaitForProcessGroupExitAsync(game.Process.Id, TimeSpan.FromMilliseconds(ProcessStopTimeoutMilliseconds), CancellationToken.None);
                return StopMode.Forced;
            }
        }
        finally
        {
            File.Delete(game.Tracker.DescriptorPath);
        }
    }

    private async Task<RunningGame> LaunchPreparedGameAsync(string minecraftDirectory, string portableMinecraftVersion, IReadOnlyList<string> arguments, int? memoryMb, CancellationToken cancellationToken)
    {
        await PrepareDisplayAndWindowAsync(cancellationToken);
        Console.Error.WriteLine($"Launching Minecraft with PortableMC version: {portableMinecraftVersion}");
        var tracker = CreateTrackerConnection(FindUsername(arguments));
        var launchArguments = memoryMb is { } value
            ? arguments.Append(CreateMaximumHeapArgument(value)).Append($"--jvm-arg=-javaagent:{PortableMinecraftAgentPath}={CreateAgentArguments(tracker)}").Cast<string?>().ToArray()
            : arguments.Append($"--jvm-arg=-javaagent:{PortableMinecraftAgentPath}={CreateAgentArguments(tracker)}").Cast<string?>().ToArray();
        var initialOutOfMemoryKillCount = CgroupMemoryEvents.ReadOutOfMemoryKillCount();
        Process process;

        try
        {
            process = LaunchPortableMinecraftClient(minecraftDirectory, portableMinecraftVersion, launchArguments, cancellationToken);
        }
        catch
        {
            File.Delete(tracker.DescriptorPath);
            throw;
        }

        try
        {
            var managedProcess = new ManagedProcess(process, memoryMb, initialOutOfMemoryKillCount);
            using var windowCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var windowTask = WaitForPreparedLargestWindowAsync(Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay, windowCancellationTokenSource.Token);
            var processExitTask = managedProcess.WaitForExitAsync(CancellationToken.None);

            if (await Task.WhenAny(windowTask, processExitTask) == processExitTask)
            {
                await windowCancellationTokenSource.CancelAsync();
                await ((Task)windowTask).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                throw new GameProcessExitException(process.ExitCode, managedProcess.WasOutOfMemoryKilled, memoryMb);
            }

            await windowTask;
            return new RunningGame(managedProcess, portableMinecraftVersion, DateTimeOffset.UtcNow, tracker);
        }
        catch
        {
            KillProcess(process);
            await WaitForKilledProcessAsync(process);
            process.Dispose();
            File.Delete(tracker.DescriptorPath);
            throw;
        }
    }

    internal static string CreateMaximumHeapArgument(int memoryMb)
    {
        return $"--jvm-arg=-Xmx{memoryMb.ToString(CultureInfo.InvariantCulture)}M";
    }

    private static GameTrackerConnection CreateTrackerConnection(string? expectedName)
    {
        var descriptorPath = Path.Combine(Path.GetTempPath(), $"void-client-agent-{Guid.NewGuid():N}.port");
        return new GameTrackerConnection(descriptorPath, Convert.ToHexString(RandomNumberGenerator.GetBytes(32)), expectedName);
    }

    private static string CreateAgentArguments(GameTrackerConnection tracker)
    {
        var arguments = $"descriptor={EncodeAgentArgument(tracker.DescriptorPath)};token={EncodeAgentArgument(tracker.Token)}";
        return tracker.ExpectedName is null ? arguments : $"{arguments};name={EncodeAgentArgument(tracker.ExpectedName)}";
    }

    private async Task AttachPlayerTrackerAsync(RunningGame game, CancellationToken cancellationToken)
    {
        var javaProcessId = FindJavaProcessId(game.Process.Id);

        if (javaProcessId is null)
            throw PlayersUnavailable("The running Minecraft JVM could not be found");

        var agentPathAndArguments = $"{PortableMinecraftAgentPath}={CreateAgentArguments(game.Tracker)}";
        var result = await RunProcessTextAsync(CreateProcessInfo(PortableMinecraftJvmAttachPath,
            [javaProcessId.Value.ToString(CultureInfo.InvariantCulture), "load", "instrument", "false", agentPathAndArguments]),
            TimeSpan.FromSeconds(10), cancellationToken);

        if (result.ExitCode is not 0)
            throw PlayersUnavailable($"The Minecraft player tracker could not attach: {result.StandardError}");
    }

    private static int? FindJavaProcessId(int rootProcessId)
    {
        var descendants = new HashSet<int> { rootProcessId };
        var processDirectories = Directory.EnumerateDirectories("/proc")
            .Select(path => (Path: path, Name: Path.GetFileName(path)))
            .Where(item => int.TryParse(item.Name, NumberStyles.None, CultureInfo.InvariantCulture, out _))
            .ToArray();
        var changed = true;

        while (changed)
        {
            changed = false;

            foreach (var (path, name) in processDirectories)
            {
                var processId = int.Parse(name, CultureInfo.InvariantCulture);

                if (descendants.Contains(processId) || !TryReadParentProcessId(path, out var parentProcessId) || !descendants.Contains(parentProcessId))
                    continue;

                descendants.Add(processId);
                changed = true;
            }
        }

        foreach (var processId in descendants.OrderDescending())
        {
            try
            {
                var arguments = File.ReadAllText($"/proc/{processId}/cmdline").Split('\0', StringSplitOptions.RemoveEmptyEntries);

                if (arguments.Any(argument => Path.GetFileName(argument) is "java" or "java-x86_64"))
                    return processId;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // The process exited while its command line was being inspected.
            }
        }

        return null;
    }

    private static bool TryReadParentProcessId(string processDirectory, out int parentProcessId)
    {
        parentProcessId = 0;

        try
        {
            foreach (var line in File.ReadLines(Path.Combine(processDirectory, "status")))
            {
                if (!line.StartsWith("PPid:", StringComparison.Ordinal))
                    continue;

                return int.TryParse(line.AsSpan("PPid:".Length).Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out parentProcessId);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // The process exited before its parent process ID could be read.
        }

        return false;
    }

    private static string EncodeAgentArgument(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string? FindUsername(IReadOnlyList<string> arguments)
    {
        for (var index = 0; index < arguments.Count; index++)
        {
            if (arguments[index] is "--username" or "-u")
                return index + 1 < arguments.Count ? arguments[index + 1] : null;

            if (arguments[index].StartsWith("--username=", StringComparison.Ordinal))
                return arguments[index]["--username=".Length..];
        }

        return null;
    }

    private static void ValidatePlayer(GamePlayer player)
    {
        if (!double.IsFinite(player.Position.X) || !double.IsFinite(player.Position.Y) || !double.IsFinite(player.Position.Z))
            throw PlayersUnavailable("The Minecraft player tracker returned a non-finite player coordinate");

        ValidateRotation(player.Body, player.Head);
    }

    private static void ValidatePlayer(RemoteGamePlayer player)
    {
        if (!double.IsFinite(player.Position.X) || !double.IsFinite(player.Position.Y) || !double.IsFinite(player.Position.Z))
            throw PlayersUnavailable("The Minecraft player tracker returned a non-finite player coordinate");

        ValidateRotation(player.Body, player.Head);
    }

    private static void ValidateRotation(BodyRotation? body, HeadRotation? head)
    {
        if (body is null || head is null)
            throw PlayersUnavailable("The Minecraft player tracker returned an incomplete player rotation");

        if (!double.IsFinite(body.Yaw) || !double.IsFinite(head.Yaw) || !double.IsFinite(head.Pitch))
            throw PlayersUnavailable("The Minecraft player tracker returned a non-finite player rotation");
    }

    private static GamePlayersException PlayersUnavailable(string message, string stage = "snapshot", Exception? innerException = null)
    {
        return new(StatusCodes.Status503ServiceUnavailable, "client.players.unavailable", stage, message, innerException);
    }

    private sealed record TrackerResponse(string? Status, string? Message, GamePlayer? Local, RemoteGamePlayer[]? Remote);

    private static string GetMinecraftDirectory()
    {
        return Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? DefaultMinecraftDirectory;
    }

    private static async Task WaitForProcessGroupExitAsync(int processGroupId, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        while (IsProcessGroupRunning(processGroupId))
        {
            if (stopwatch.Elapsed >= timeout)
                throw new TimeoutException($"Minecraft process group did not exit within {timeout.TotalSeconds:F1} seconds");

            await Task.Delay(50, cancellationToken);
        }
    }

    private static bool IsProcessGroupRunning(int processGroupId)
    {
        foreach (var processDirectory in Directory.EnumerateDirectories("/proc"))
        {
            try
            {
                var status = File.ReadAllText(Path.Combine(processDirectory, "stat"));
                var commandEnd = status.LastIndexOf(')');

                if (commandEnd < 0)
                    continue;

                var fields = status[(commandEnd + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (fields.Length > 2 && fields[0] is not "Z" && int.TryParse(fields[2], NumberStyles.None, CultureInfo.InvariantCulture, out var candidateProcessGroupId) && candidateProcessGroupId == processGroupId)
                    return true;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // The process exited while its state was being inspected.
            }
        }

        return false;
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

        if (_chatInputTargetsWindow)
            await PasteTextAsync(inputWindowId, message, display, cancellationToken);
        else
            await TypeTextAsync(inputWindowId, message, _chatInputSupportsVisualConfirmation ? 50 : 150, cancellationToken);

        await SubmitChatAsync(windowId, inputWindowId, display, cancellationToken);
    }

    async Task JoinServerOperationAsync(string host, int port, CancellationToken cancellationToken)
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? DefaultDisplay;
        var windowLease = await AcquirePreparedWindowLeaseAsync(display, cancellationToken);
        var serverAddress = $"{host}:{port}";
        var stage = "screen.capture";
        ConnectionNavigationKind? clickedAction = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                stage = "screen.prepare";
                await MoveMouseAsync(windowLease.Id, 2, 2, cancellationToken);
                stage = "screen.capture";
                using var screen = await CaptureScreenImageAsync(windowLease.Id, display, cancellationToken);
                stage = "screen.recognition";
                var recognition = await RecognizeConnectionScreenAsync(screen, cancellationToken);
                var observation = recognition.Observation;

                if (observation is null)
                {
                    stage = "game.confirmation";
                    if (recognition.Matches.Count is 0 && await TryConfirmInteractiveGameScreenAsync(windowLease.Id, display, cancellationToken))
                        return;

                    continue;
                }

                if (clickedAction == observation.Kind)
                {
                    Console.Error.WriteLine($"OCR still sees {observation.Kind} after its click; waiting for a different recognized action");
                    continue;
                }

                LogRecognizedConnectionAction(observation.Kind, observation.TextMatch);

                if (observation.Kind is ConnectionNavigationKind.Back)
                    Console.Error.WriteLine("Visually confirmed that the connection failed; returning to the server list to retry");

                if (observation.Kind is ConnectionNavigationKind.JoinServer)
                {
                    stage = "address.entry";
                    var serverAddressMatch = observation.ServerAddressMatch
                                             ?? throw new InvalidOperationException("The Join Server action has no recognized Server Address field");
                    var joinServerMatch = await EnterServerAddressAsync(windowLease.Id, display, serverAddress, serverAddressMatch, cancellationToken);

                    stage = "connection.submit";
                    LogRecognizedConnectionAction(observation.Kind, joinServerMatch);
                    await ClickRecognizedConnectionActionAsync(windowLease.Id, observation.Kind, joinServerMatch, cancellationToken);
                }
                else
                {
                    stage = "action.click";
                    await ClickRecognizedConnectionActionAsync(windowLease.Id, observation.Kind, observation.TextMatch, cancellationToken);
                }

                clickedAction = observation.Kind;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (GameClientException)
            {
                throw;
            }
            catch (Exception exception) when (exception is ExternalProcessException or InvalidOperationException)
            {
                var staleWindowFailure = await GetStaleWindowFailureAsync(windowLease, display, exception, cancellationToken);

                if (staleWindowFailure is not null)
                {
                    windowLease = await AcquirePreparedWindowLeaseAsync(display, cancellationToken);
                    Console.Error.WriteLine($"{staleWindowFailure.Message} during {stage}; acquired lease {windowLease.Generation} ({windowLease.Id}) and restarted recognition");
                    continue;
                }

                throw new GameClientException("client.connect.failed", "connect", stage, $"Minecraft client connection failed during {stage}: {exception.Message}", exception);
            }
            catch (Exception exception)
            {
                throw new GameClientException("client.connect.failed", "connect", stage, $"Minecraft client connection failed during {stage}: {exception.Message}", exception);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    async Task<ConnectionScreenRecognition> RecognizeConnectionScreenAsync(ScreenImage screen, CancellationToken cancellationToken)
    {
        var recognizedTexts = await _textRecognizer.RecognizeAsync(screen.Bytes, OcrModelTier.Small, cancellationToken);
        var matches = ConnectionTextMatcher.Match(recognizedTexts);
        var observation = CreateConnectionScreenObservation(matches);

        if (ConnectionOcrFallbackPolicy.IsReliable(observation?.TextMatch))
            return new(matches, observation);

        Console.Error.WriteLine("Small OCR did not find a reliable connection action; retrying the current screen with medium OCR");
        recognizedTexts = await _textRecognizer.RecognizeAsync(screen.Bytes, OcrModelTier.Medium, cancellationToken);
        matches = ConnectionTextMatcher.Match(recognizedTexts);
        observation = CreateConnectionScreenObservation(matches);
        return new(matches, observation);
    }

    static ConnectionScreenObservation? CreateConnectionScreenObservation(IReadOnlyDictionary<ConnectionTextAction, ConnectionTextMatch> matches)
    {
        var selection = ConnectionNavigationSelector.Select(matches);

        if (selection is null || !matches.TryGetValue(selection.TextAction, out var match))
            return null;

        matches.TryGetValue(ConnectionTextAction.ServerAddress, out var serverAddressMatch);
        return new(selection.Kind, match, serverAddressMatch);
    }

    async Task ClickRecognizedConnectionActionAsync(string windowId, ConnectionNavigationKind kind, ConnectionTextMatch match, CancellationToken cancellationToken)
    {
        await FocusMoveAndClickAsync(windowId, match.Bounds, cancellationToken);
        Console.Error.WriteLine($"Clicked OCR-selected {kind} '{match.Text}' at {match.Bounds}");
    }

    static void LogRecognizedConnectionAction(ConnectionNavigationKind kind, ConnectionTextMatch match)
    {
        Console.Error.WriteLine($"OCR selected {kind} '{match.Text}' at {match.Bounds} with {match.Confidence:P1} confidence and {match.Similarity:P1} similarity");
    }

    async Task<bool> TryConfirmInteractiveGameScreenAsync(string windowId, string display, CancellationToken cancellationToken)
    {
        var preferredInputWindowId = _chatInputTargetsWindow ? windowId : null;

        if (_chatInputSupportsVisualConfirmation)
        {
            if (await TryOpenAndCloseChatAsync(windowId, preferredInputWindowId, display, pressKeySlowly: false, key: "t", cancellationToken: cancellationToken))
                return true;

            if (preferredInputWindowId is not null && await TryOpenAndCloseChatAsync(windowId, null, display, pressKeySlowly: false, key: "t", cancellationToken: cancellationToken))
                return true;

            return false;
        }

        return await TryOpenAndCloseChatAsync(windowId, null, display, pressKeySlowly: true, key: "t", cancellationToken: cancellationToken)
            || await TryOpenAndCloseChatAsync(windowId, null, display, pressKeySlowly: true, key: "slash", cancellationToken: cancellationToken);
    }

    async Task<bool> TryOpenAndCloseChatAsync(string windowId, string? inputWindowId, string display, bool pressKeySlowly, string key, CancellationToken cancellationToken)
    {
        if (pressKeySlowly)
            await PressKeySlowlyAsync(inputWindowId, key, cancellationToken);
        else
            await PressKeyAsync(inputWindowId, key, cancellationToken);

        if (!await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            return false;

        await ClearChatInputAsync(pressKeysSlowly: pressKeySlowly, cancellationToken: cancellationToken);

        if (!await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            return false;

        await PressKeyAsync(null, "Escape", cancellationToken);

        while (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            cancellationToken.ThrowIfCancellationRequested();

        windowId = await ResumeGameIfPausedAsync(windowId, display, cancellationToken);

        Console.Error.WriteLine("Visually confirmed an interactive in-game screen");
        return true;
    }

    async Task<string> ResumeGameIfPausedAsync(string windowId, string display, CancellationToken cancellationToken)
    {
        while (true)
        {
            using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
            var recognizedTexts = await _textRecognizer.RecognizeAsync(screen.Bytes, OcrModelTier.Small, cancellationToken);
            var matches = ConnectionTextMatcher.Match(recognizedTexts);

            if (!matches.TryGetValue(ConnectionTextAction.BackToGame, out var backToGame))
                return windowId;

            Console.Error.WriteLine("OCR visually confirmed the pause menu; returning to the game");
            LogRecognizedConnectionAction(ConnectionNavigationKind.BackToGame, backToGame);
            await ClickRecognizedConnectionActionAsync(windowId, ConnectionNavigationKind.BackToGame, backToGame, cancellationToken);
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
        return (await AcquirePreparedWindowLeaseAsync(display, cancellationToken)).Id;
    }

    async Task<MinecraftWindowLease> AcquirePreparedWindowLeaseAsync(string display, CancellationToken cancellationToken)
    {
        while (true)
        {
            var windowId = await WaitForLargestWindowAsync(display, cancellationToken);
            var lease = new MinecraftWindowLease(windowId, Interlocked.Increment(ref _nextWindowGeneration));

            try
            {
                await ResizeWindowToDisplayAsync(windowId, cancellationToken);
                await RunOrThrow(cancellationToken, "xdotool", "windowfocus", windowId);
                return lease;
            }
            catch (Exception exception) when (exception is ExternalProcessException or InvalidOperationException)
            {
                var staleWindowFailure = await GetStaleWindowFailureAsync(lease, display, exception, cancellationToken);

                if (staleWindowFailure is not null)
                {
                    Console.Error.WriteLine($"{staleWindowFailure.Message} while preparing the window; reacquiring");
                    continue;
                }

                throw new GameClientException("client.window.prepare.failed", "window", "prepare", $"Preparing Minecraft window {windowId} failed: {exception.Message}", exception);
            }
        }
    }

    async Task<StaleMinecraftWindowException?> GetStaleWindowFailureAsync(MinecraftWindowLease lease, string display, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ExternalProcessException processException && X11FailureClassifier.IsExplicitStaleWindow(processException))
            return new(lease, exception);

        try
        {
            var processInfo = CreateProcessInfo("xdotool", ["getwindowgeometry", "--shell", lease.Id], display: display);
            var result = await RunProcessTextAsync(processInfo, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);
            return result.ExitCode is 0 ? null : new(lease, exception);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            // A failed probe is not proof that the original error was a stale window. Preserve the original fault.
            return null;
        }
    }

    async Task<ConnectionTextMatch> EnterServerAddressAsync(string windowId, string display, string serverAddress, ConnectionTextMatch serverAddressMatch, CancellationToken cancellationToken)
    {
        Console.Error.WriteLine($"OCR confirmed Server Address '{serverAddressMatch.Text}' at {serverAddressMatch.Bounds} with {serverAddressMatch.Confidence:P1} confidence and {serverAddressMatch.Similarity:P1} similarity");
        var serverAddressInputTarget = new OcrRectangle(
            serverAddressMatch.Bounds.CenterX,
            serverAddressMatch.Bounds.Bottom + serverAddressMatch.Bounds.Height,
            1,
            1);
        await FocusMoveAndClickAsync(windowId, serverAddressInputTarget, cancellationToken);
        Console.Error.WriteLine($"Focused the input associated with OCR-recognized Server Address at {serverAddressInputTarget}");
        await ClearTextWithoutDelayAsync(cancellationToken);
        await TypeTextWithoutDelayAsync(null, serverAddress, cancellationToken);

        while (true)
        {
            using var screen = await CaptureScreenImageAsync(windowId, display, cancellationToken);
            var recognizedTexts = await _textRecognizer.RecognizeAsync(screen.Bytes, OcrModelTier.Small, cancellationToken);
            var matches = ConnectionTextMatcher.Match(recognizedTexts);

            if (matches.ContainsKey(ConnectionTextAction.ServerAddress)
                && matches.TryGetValue(ConnectionTextAction.JoinServer, out var joinServerMatch))
            {
                Console.Error.WriteLine("OCR confirmed the direct connection form after server address entry");
                return joinServerMatch;
            }

            Console.Error.WriteLine("OCR has not confirmed the direct connection form and Join Server action after address entry; rescanning");
        }
    }

    async Task<ScreenImage> CaptureScreenImageAsync(string windowId, string display, CancellationToken cancellationToken)
    {
        await Task.Delay(UserInterfacePollDelayMilliseconds, cancellationToken);

        var captureResult = await RunScreenCaptureBytesAsync(
            currentWindowId => CreateProcessInfo("import", ["-window", currentWindowId, "-depth", "8", "ppm:-"], display: display),
            windowId,
            display,
            cancellationToken);

        if (captureResult.ExitCode is not 0)
            throw new InvalidOperationException($"screen analysis capture failed: {captureResult.StandardError}");

        return ScreenImage.LoadPortablePixmap(captureResult.StandardOutput);
    }

    async Task MoveMouseAsync(string windowId, int x, int y, CancellationToken cancellationToken)
    {
        await RunOrThrow(cancellationToken, "xdotool", "windowfocus", windowId);
        await RunOrThrow(cancellationToken, "xdotool", "mousemove", "--window", windowId, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture));
    }

    async Task FocusMoveAndClickAsync(string windowId, OcrRectangle target, CancellationToken cancellationToken)
    {
        await RunOrThrow(
            cancellationToken,
            "xdotool",
            "windowfocus", windowId,
            "mousemove", "--window", windowId, "2", "2",
            "mousemove", "--sync", "--window", windowId, target.CenterX.ToString(CultureInfo.InvariantCulture), target.CenterY.ToString(CultureInfo.InvariantCulture),
            "click", "1");
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
        await RunOrThrow(cancellationToken, "xset", "r", "off");

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

        var captureResult = await RunScreenCaptureBytesAsync(
            currentWindowId => CreateProcessInfo("import", ["-window", currentWindowId, "png:-"], display: display),
            windowId,
            display,
            cancellationToken);

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

    async Task PasteTextAsync(string? windowId, string text, string display, CancellationToken cancellationToken = default)
    {
        var processInfo = CreateProcessInfo("xclip", ["-selection", "clipboard", "-in", "-loops", "2", "-quiet"], display: display);
        processInfo.RedirectStandardInput = true;
        processInfo.RedirectStandardOutput = false;
        processInfo.RedirectStandardError = false;

        using var process = Process.Start(processInfo)
                            ?? throw new InvalidOperationException("failed to start xclip");

        try
        {
            await process.StandardInput.WriteAsync(text.AsMemory(), cancellationToken);
            process.StandardInput.Close();
            var clipboardDeadline = DateTimeOffset.UtcNow.AddMilliseconds(ExternalProcessTimeoutMilliseconds);
            var clipboardMatches = false;

            while (!clipboardMatches && DateTimeOffset.UtcNow < clipboardDeadline)
            {
                var clipboardResult = await RunProcessTextAsync(CreateProcessInfo("xclip", ["-selection", "clipboard", "-out"], display: display), TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);
                clipboardMatches = clipboardResult.ExitCode is 0 && clipboardResult.StandardOutput == text;

                if (!clipboardMatches)
                    await Task.Delay(25, cancellationToken);
            }

            if (!clipboardMatches)
                throw new InvalidOperationException("xclip did not acquire the clipboard selection");

            await PressKeyAsync(windowId, "ctrl+v", cancellationToken);
            await WaitForProcessExitAsync(process, processInfo.FileName, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);
        }
        catch
        {
            KillProcess(process);
            await WaitForKilledProcessAsync(process);
            throw;
        }

        if (process.ExitCode is not 0)
            throw new InvalidOperationException($"xclip exited with code {process.ExitCode}");
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

            while (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
                cancellationToken.ThrowIfCancellationRequested();

            return;
        }

        await PressKeyAsync(inputWindowId, "Return", cancellationToken);

        while (await IsChatInputVisibleAsync(windowId, display, cancellationToken))
            cancellationToken.ThrowIfCancellationRequested();
    }

    async Task<bool> IsChatInputVisibleAsync(string windowId, string display, CancellationToken cancellationToken = default)
    {
        await Task.Delay(UserInterfacePollDelayMilliseconds, cancellationToken);

        var captureResult = await RunScreenCaptureBytesAsync(
            currentWindowId => CreateProcessInfo(
                "import",
                ["-window", currentWindowId, "-crop", ChatInputBrightnessCropGeometry, "+repage", "-depth", "8", "ppm:-"],
                display: display),
            windowId,
            display,
            cancellationToken);

        if (captureResult.ExitCode != 0)
            throw new InvalidOperationException($"chat input capture failed: {captureResult.StandardError}");

        using var screen = ScreenImage.LoadPortablePixmap(captureResult.StandardOutput);
        return screen.IsChatInputVisible();
    }

    async Task<string?> FindLargestWindow(string display, CancellationToken cancellationToken = default)
    {
        var searchProcessInfo = CreateProcessInfo("xdotool", ["search", "--onlyvisible", "--name", ".*"], display: display);
        var searchResult = await RunProcessTextAsync(searchProcessInfo, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);

        if (searchResult.ExitCode != 0)
            return null;

        string? largestWindowId = null;
        long largestArea = 0;

        foreach (var candidateWindowId in searchResult.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedCandidateId = candidateWindowId.Trim();
            var nameProcessInfo = CreateProcessInfo("xdotool", ["getwindowname", trimmedCandidateId], display: display);
            var nameResult = await RunProcessTextAsync(nameProcessInfo, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);

            if (nameResult.ExitCode != 0 || string.IsNullOrWhiteSpace(nameResult.StandardOutput))
                continue;

            var geometryProcessInfo = CreateProcessInfo("xdotool", ["getwindowgeometry", "--shell", trimmedCandidateId], display: display);
            var geometryResult = await RunProcessTextAsync(geometryProcessInfo, TimeSpan.FromMilliseconds(ExternalProcessTimeoutMilliseconds), cancellationToken);

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
            throw new ExternalProcessException(command[0], command[1..], result.ExitCode, result.StandardOutput, result.StandardError);
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

    async Task<ProcessBytesResult> RunScreenCaptureBytesAsync(Func<string, ProcessStartInfo> createProcessInfo, string windowId, string display, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= ScreenCaptureMaximumAttempts; attempt++)
        {
            try
            {
                return await RunProcessBytesAsync(createProcessInfo(windowId), TimeSpan.FromMilliseconds(ScreenCaptureTimeoutMilliseconds), cancellationToken);
            }
            catch (TimeoutException exception) when (attempt < ScreenCaptureMaximumAttempts)
            {
                Console.Error.WriteLine($"{exception.Message}; reacquiring the Minecraft window before screen capture attempt {attempt + 1}");
                windowId = await WaitForPreparedLargestWindowAsync(display, cancellationToken);
            }
        }

        throw new InvalidOperationException("Screen capture attempts were exhausted");
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
