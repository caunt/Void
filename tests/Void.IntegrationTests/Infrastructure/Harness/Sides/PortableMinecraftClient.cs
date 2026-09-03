using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Void.IntegrationTests.Infrastructure.Exceptions;
using Void.IntegrationTests.Infrastructure.Extensions;
using Void.Minecraft.Network;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PortableMinecraftClient(IContainer Container, HttpClient HttpClient) : IIntegrationSide
{
    private const int ApiPort = 80;
    private const int ClientStatePollDelayMilliseconds = 250;
    private const string Display = ":99";
    private const string DockerHost = "host.docker.internal";
    private const string DockerHostGateway = "host-gateway";
    private const string LogMessagePrefix = $"[{nameof(Void)}.{nameof(IntegrationTests)}]";

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.Oldest, ProtocolVersion.Latest);

    public string LogFileName => "client.log";
    public IEnumerable<string> Logs => ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

    public void ClearLogs()
    {
        _readLogsSince = DateTime.UtcNow;
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await Container.ReadLogsAsync(since, cancellationToken);
    }

    public static async Task<PortableMinecraftClient> CreateAsync(CancellationToken cancellationToken = default)
    {
        var builder = new ContainerBuilder("ghcr.io/void-community/portable-minecraft-client:offline")
            .WithEnvironment("DISPLAY", Display)
            .WithPortBinding(port: ApiPort, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(ApiPort)
                    .ForPath("/api/health"), options => options.WithTimeout(TimeSpan.FromMinutes(1))));

        if (OperatingSystem.IsLinux())
            builder = builder.WithExtraHost(DockerHost, DockerHostGateway);

        var container = builder.Build();

        await container.StartAsync(cancellationToken);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{container.Hostname}:{container.GetMappedPublicPort(ApiPort)}"),
            Timeout = Timeouts.SetupTimeout
        };

        return new PortableMinecraftClient(container, httpClient);
    }

    public async Task<Game> RunGameAsync(string testName, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
    {
        return await RunGameAsync(testName, protocolVersion, [], cancellationToken);
    }

    public async Task<Game> RunGameAsync(string testName, ProtocolVersion protocolVersion, IEnumerable<IIntegrationSide> additionalLogSides, CancellationToken cancellationToken = default)
    {
        return await Game.RunAsync(testName, Container, HttpClient, [this, .. additionalLogSides], protocolVersion, cancellationToken);
    }

    public record Game(string TestName, IContainer Container, HttpClient HttpClient, IReadOnlyList<IIntegrationSide> LogSides, DateTime StartedAt, ProtocolVersion ProtocolVersion, string Username) : IAsyncDisposable
    {
        private readonly string _workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "steps", TestName, ProtocolVersion.ToString(), Username);
        private ApiProblemDetails? _lastPlayersProblem;
        private int _step;

        public async ValueTask DisposeAsync()
        {
            await LogAsync($"Stopping Minecraft {ProtocolVersion.FirstRelease}", Timeouts.StepTimeoutToken);

            try
            {
                try
                {
                    await TryMakeStepAsync("exit", Timeouts.StepTimeoutToken);
                }
                finally
                {
                    await TryWriteLogsAsync(Timeouts.StepTimeoutToken);
                }
            }
            finally
            {
                await StopClientAsync(Timeouts.StepTimeoutToken);
            }

            GC.SuppressFinalize(this);
        }

        public static async Task<Game> RunAsync(string testName, IContainer container, HttpClient httpClient, IReadOnlyList<IIntegrationSide> logSides, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
        {
            var username = Convert.ToHexString(BitConverter.GetBytes(Random.Shared.Next()));
            var startedAt = DateTime.UtcNow;

            using var optionsContent = new StringContent(CreateOptionsText(protocolVersion), Encoding.UTF8, "text/plain");
            using var optionsResponse = await httpClient.PutAsync("/api/game/options", optionsContent, cancellationToken);
            await EnsureSuccessAsync(optionsResponse, $"Writing Minecraft {protocolVersion.FirstRelease} options", cancellationToken);

            var game = new Game(testName, container, httpClient, logSides, startedAt, protocolVersion, username);

            try
            {
                await game.LogAsync($"Starting Minecraft {protocolVersion.FirstRelease}", cancellationToken);
                await game.StartVanillaAsync(cancellationToken);
            }
            catch
            {
                await game.TryWriteLogsAsync(Timeouts.StepTimeoutToken);
                await game.StopClientAsync(Timeouts.StepTimeoutToken);
                throw;
            }

            return game;
        }

        public async Task SendTextMessageAsync(string text, CancellationToken cancellationToken = default)
        {
            await SendTextMessagesAsync([text], cancellationToken);
        }

        public async Task SendTextMessagesAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
        {
            foreach (var text in texts)
            {
                // Is it a Chat Command?
                var expectTask = text.StartsWith('/')
                    ? Task.Delay(3_000, cancellationToken) // Give some room for client-server reaction
                    : Container.ExpectTextAsync(text, cancellationToken); // Expect the text to appear in logs

                await LogAsync($"Sending chat input: {text}", cancellationToken);
                await SendChatAsync(text, cancellationToken);
                await expectTask;

                await MakeStepAsync("chat", cancellationToken);
            }
        }

        public async Task EnsureStableAsync(CancellationToken cancellationToken = default)
        {
            await EnsureStableAsync(TimeSpan.FromSeconds(10), cancellationToken);
        }

        public async Task EnsureStableAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            var timestamp = Stopwatch.GetTimestamp();
            await LogAsync($"Waiting for logs to become stable for {duration.TotalSeconds:F2} seconds", cancellationToken);

            // Ensure the game is stable and not doing some background loading
            await Container.WaitForLogsSilenceAsync(duration, whitelist:
            [
                // Ignore log messages
                LogMessagePrefix,

                // Ignore chat messages (might be many clients on a single server)
                "[CHAT]",

                // Ignore spam messages that can appear because of poor ViaVersion protocol support
                "Unable to play unknown soundEvent", // Unable to play unknown soundEvent minecraft:mob.rabbit.hop (or .idle)
                " has no item?!", // Item entity 72 has no item?!
                "Skipping Entity with id" // Skipping Entity with id minecraft:cave_spider
            ], cancellationToken);

            var remainingDuration = duration - Stopwatch.GetElapsedTime(timestamp);

            if (remainingDuration > TimeSpan.Zero)
                await Task.Delay(remainingDuration, cancellationToken);

            await LogAsync($"Logs are stable after {Stopwatch.GetElapsedTime(timestamp).TotalSeconds:F2} seconds", cancellationToken);
            await MakeStepAsync("stable", cancellationToken);
        }

        public async Task<ApiGamePlayers?> TryReadPlayersAsync(CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync("/api/game/players", cancellationToken);

            if (response.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.ServiceUnavailable)
            {
                _lastPlayersProblem = await ReadProblemDetailsAsync(response, cancellationToken);
                return null;
            }

            await EnsureSuccessAsync(response, "Reading Minecraft players", cancellationToken);
            _lastPlayersProblem = null;
            return await response.Content.ReadFromJsonAsync<ApiGamePlayers>(cancellationToken)
                   ?? throw new IntegrationTestException("Reading Minecraft players returned an empty response");
        }

        public async Task<string> ReadDiagnosticsAsync()
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            try
            {
                using var response = await HttpClient.GetAsync("/api/game/status", cancellation.Token);
                var statusContent = await response.Content.ReadAsStringAsync(cancellation.Token);
                return JsonSerializer.Serialize(new
                {
                    version = ProtocolVersion.FirstRelease.ToString(),
                    username = Username,
                    statusCode = (int)response.StatusCode,
                    status = TryDeserialize<ApiStatus>(statusContent),
                    rawStatus = statusContent,
                    lastPlayersProblem = _lastPlayersProblem
                }, new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });
            }
            catch (Exception exception)
            {
                return $"Failed to read Minecraft client diagnostics: {exception}";
            }
        }

        private async Task StartVanillaAsync(CancellationToken cancellationToken = default)
        {
            var memoryMb = ProtocolVersion >= ProtocolVersion.MINECRAFT_26_1 ? 4096 : 2048;
            var request = new
            {
                version = ProtocolVersion.FirstRelease.ToString(),
                memoryMb,
                arguments = new[]
                {
                    "--fetch-exclude-all",
                    "--username",
                    Username,
                    "--jvm-arg=-Djava.awt.headless=false"
                }
            };

            using var response = await HttpClient.PostAsJsonAsync("/api/game/start/vanilla", request, cancellationToken);
            await EnsureSuccessAsync(response, $"Starting Minecraft {ProtocolVersion.FirstRelease}", cancellationToken);
            var acceptedStatus = await ReadApiStatusAsync(response, $"Accepting Minecraft {ProtocolVersion.FirstRelease} launch", cancellationToken);

            await WaitForClientReadyAsync(acceptedStatus.OperationId, cancellationToken);
        }

        public async Task JoinServerAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
        {
            var (dockerHost, dockerPort) = endPoint.AsDockerHostPort;

            await LogAsync($"Navigating the Minecraft UI to {dockerHost}:{dockerPort}", cancellationToken);

            using var response = await HttpClient.PostAsJsonAsync("/api/game/connect", new { host = dockerHost, port = dockerPort }, cancellationToken);
            await EnsureSuccessAsync(response, $"Navigating Minecraft {ProtocolVersion.FirstRelease} to {dockerHost}:{dockerPort}", cancellationToken);
        }

        public async Task JoinServerExpectingFailureAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
        {
            using var connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var connectionTask = JoinServerAsync(endPoint, connectionCancellation.Token);

            try
            {
                await Container.ExpectTextAsync("Visually confirmed that the connection failed", StartedAt, cancellationToken);
            }
            catch
            {
                await connectionCancellation.CancelAsync();
                await connectionTask.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                throw;
            }

            await connectionCancellation.CancelAsync();

            try
            {
                await connectionTask;
                throw new IntegrationTestException($"Minecraft {ProtocolVersion.FirstRelease} completed its connection attempt when a connection failure was expected");
            }
            catch (OperationCanceledException) when (connectionCancellation.IsCancellationRequested)
            {
                // Cancellation is the expected outcome after the connection-failure screen was confirmed.
            }

            await LogAsync($"Minecraft {ProtocolVersion.FirstRelease} visually confirmed the expected connection failure", cancellationToken);
        }

        private async Task SendChatAsync(string text, CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.PostAsJsonAsync("/api/game/send-chat", new { message = text }, cancellationToken);
            await EnsureSuccessAsync(response, $"Sending chat input: {text}", cancellationToken);
        }

        private async Task StopClientAsync(CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.PostAsync("/api/game/stop", null, cancellationToken);
            await EnsureSuccessAsync(response, $"Stopping Minecraft {ProtocolVersion.FirstRelease}", cancellationToken);
            var stopResponse = await response.Content.ReadFromJsonAsync<StopGameResponse>(cancellationToken)
                               ?? throw new IntegrationTestException("Stopping Minecraft returned an empty response");

            if (stopResponse.Status.State is not "idle" || stopResponse.Status.OperationState is not "succeeded")
                throw new IntegrationTestException($"Stopping Minecraft returned unexpected state {stopResponse.Status.State}/{stopResponse.Status.OperationState}");
        }

        private async Task WaitForClientReadyAsync(long operationId, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                using var response = await HttpClient.GetAsync("/api/game/status", cancellationToken);
                await EnsureSuccessAsync(response, "Reading Minecraft client status", cancellationToken);

                var status = await ReadApiStatusAsync(response, "Reading Minecraft client status", cancellationToken);

                if (status.OperationId != operationId)
                    throw new IntegrationTestException($"Minecraft launch operation {operationId} was superseded by {status.OperationId}");

                if (status.State is "ready" && status.OperationState is "succeeded")
                    return;

                if (status.State is "failed" || status.OperationState is "failed" or "canceled")
                    throw new IntegrationTestException($"Starting Minecraft {ProtocolVersion.FirstRelease} failed: {status.Error ?? "client entered failed state"}");

                await Task.Delay(TimeSpan.FromMilliseconds(ClientStatePollDelayMilliseconds), cancellationToken);
            }
        }

        private async Task<byte[]> TakeScreenshotAsync(CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync("/api/game/screenshot", cancellationToken);
            await EnsureSuccessAsync(response, "Taking Minecraft screenshot", cancellationToken);

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken = default)
        {
            if (response.IsSuccessStatusCode)
                return;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new IntegrationTestException($"{operation} failed with HTTP {(int)response.StatusCode} {response.ReasonPhrase}\n{content}");
        }

        private static async Task<ApiStatus> ReadApiStatusAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken = default)
        {
            return await TryReadApiStatusAsync(response, cancellationToken)
                ?? throw new IntegrationTestException($"{operation} returned an empty or malformed status response");
        }

        private static async Task<ApiStatus?> TryReadApiStatusAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
                return null;

            try
            {
                return JsonSerializer.Deserialize<ApiStatus>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static async Task<ApiProblemDetails?> ReadProblemDetailsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return TryDeserialize<ApiProblemDetails>(content)
                   ?? new(response.ReasonPhrase, (int)response.StatusCode, content, null);
        }

        private static T? TryDeserialize<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
            catch (JsonException)
            {
                return default;
            }
        }

        private Task LogAsync(string message, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"{LogMessagePrefix} {message}");
            return Task.CompletedTask;
        }

        private async Task MakeStepAsync(string action, CancellationToken cancellationToken = default)
        {
            _ = Directory.CreateDirectory(_workingDirectory);
            await File.WriteAllBytesAsync(Path.Combine(_workingDirectory, $"step-{++_step}-{action}.png"), await TakeScreenshotAsync(cancellationToken), cancellationToken);
        }

        private async Task TryMakeStepAsync(string action, CancellationToken cancellationToken = default)
        {
            try
            {
                await MakeStepAsync(action, cancellationToken);
            }
            catch (Exception exception)
            {
                await LogAsync($"Failed to write {action} step: {exception.Message}", CancellationToken.None);
            }
        }

        private async Task TryWriteLogsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteLogsAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                await LogAsync($"Failed to write step logs: {exception.Message}", CancellationToken.None);
            }
        }

        private async Task WriteLogsAsync(CancellationToken cancellationToken = default)
        {
            _ = Directory.CreateDirectory(_workingDirectory);

            foreach (var side in LogSides)
            {
                var logs = await side.ReadLogsAsync(StartedAt, cancellationToken);
                await File.WriteAllLinesAsync(Path.Combine(_workingDirectory, side.LogFileName), logs, cancellationToken);
            }
        }

        private static string CreateOptionsText(ProtocolVersion protocolVersion)
        {
            var options = new List<string>
            {
                "maxFps:15",
                "renderDistance:2",
                "pauseOnLostFocus:false"
            };

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_4)
                options.Insert(2, "onboardAccessibility:false");

            return string.Join('\n', options) + "\n";
        }

        private record ApiFailure(string Code, string Operation, string Stage, string Message, string ExceptionType, string StackTrace);

        private record ApiProblemDetails(string? Title, int? Status, string? Detail, ApiFailure? Failure);

        private record ApiStatus(string State, long OperationId, string? Operation, string OperationState, int? ProcessId, int? ExitCode, string? Message, string? Error, ApiFailure? Failure, DateTimeOffset UpdatedAt);

        private record StopGameResponse(string Mode, ApiStatus Status);

        public record ApiPosition(double X, double Y, double Z);

        public record ApiBodyRotation(double Yaw);

        public record ApiHeadRotation(double Yaw, double Pitch);

        public record ApiGamePlayer(string? Uuid, string? Name, ApiPosition Position, ApiBodyRotation Body, ApiHeadRotation Head);

        public record ApiRemoteGamePlayer(string? Uuid, string? Name, ApiPosition Position, ApiBodyRotation Body, ApiHeadRotation Head);

        public record ApiGamePlayers(ApiGamePlayer Local, IReadOnlyList<ApiRemoteGamePlayer> Remote);
    }
}
