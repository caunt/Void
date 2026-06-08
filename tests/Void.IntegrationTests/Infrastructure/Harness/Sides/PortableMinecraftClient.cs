using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    private const int SetupRetries = 5;
    private const int ApiPort = 8080;
    private const int ClientStatePollDelayMilliseconds = 250;
    private const string Display = ":99";
    private const string DockerHost = "host.docker.internal";
    private const string DockerHostGateway = "host-gateway";
    private const string LogMessagePrefix = $"[{nameof(Void)}.{nameof(IntegrationTests)}]";

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion
        .Range(ProtocolVersion.Oldest, ProtocolVersion.Latest)
        // These do connect to the server before the loading screen completes, causing MC-228828 crash on 1.14-1.19
        .Where(version => version < ProtocolVersion.MINECRAFT_1_14 || version > ProtocolVersion.MINECRAFT_1_19);

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

    public static async Task<PortableMinecraftClient> CreateAsync(CancellationToken cancellationToken = default)
    {
        var builder = new ContainerBuilder("ghcr.io/caunt/portable-minecraft-client:offline")
            .WithEnvironment("DISPLAY", Display)
            .WithPortBinding(port: ApiPort, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(ApiPort)
                    .ForPath("/health"), options => options.WithTimeout(TimeSpan.FromMinutes(1))))
            .WithCreateParameterModifier(parameters => parameters.Platform = "linux/amd64");

        if (OperatingSystem.IsLinux())
            builder = builder.WithExtraHost(DockerHost, DockerHostGateway);

        var container = builder.Build();

        await container.StartAsync(cancellationToken);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{container.Hostname}:{container.GetMappedPublicPort(ApiPort)}")
        };

        return new PortableMinecraftClient(container, httpClient);
    }

    public async Task<Game> RunGameAsync(string testName, EndPoint endPoint, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
    {
        return await RunGameAsync(testName, endPoint, protocolVersion, [], cancellationToken);
    }

    public async Task<Game> RunGameAsync(string testName, EndPoint endPoint, ProtocolVersion protocolVersion, IEnumerable<IIntegrationSide> additionalLogSides, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= SetupRetries; attempt++)
        {
            try
            {
                return await Game.RunAsync(testName, Container, HttpClient, [this, .. additionalLogSides], endPoint, protocolVersion, cancellationToken);
            }
            catch (OperationCanceledException) when (attempt < SetupRetries)
            {
                // Ignored
            }
        }

        throw new TimeoutException($"Failed to setup {protocolVersion} after {SetupRetries} attempts");
    }

    public async Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await Container.ReadLogsAsync(since, cancellationToken);
    }

    public record Game(string TestName, IContainer Container, HttpClient HttpClient, IReadOnlyList<IIntegrationSide> LogSides, DateTime StartedAt, ProtocolVersion ProtocolVersion, string Username) : IAsyncDisposable
    {
        private readonly string _workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "steps", TestName, ProtocolVersion.ToString(), Username);
        private int _step;

        public async ValueTask DisposeAsync()
        {
            await LogAsync($"Stopping Minecraft {ProtocolVersion.FirstRelease}", Timeouts.StepTimeoutToken);

            try
            {
                try
                {
                    await MakeStepAsync("exit", Timeouts.StepTimeoutToken);
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

        public static async Task<Game> RunAsync(string testName, IContainer container, HttpClient httpClient, IReadOnlyList<IIntegrationSide> logSides, EndPoint endPoint, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
        {
            var (dockerHost, dockerPort) = endPoint.AsDockerHostPort;
            var username = Convert.ToHexString(BitConverter.GetBytes(Random.Shared.Next()));
            var startedAt = DateTime.UtcNow;

            await container.CopyAsync(Encoding.UTF8.GetBytes(CreateOptionsText(protocolVersion)), "/root/.minecraft/options.txt", ct: cancellationToken);

            var game = new Game(testName, container, httpClient, logSides, startedAt, protocolVersion, username);

            try
            {
                await game.LogAsync($"Starting Minecraft {protocolVersion.FirstRelease}", cancellationToken);
                await game.StartVanillaAsync(dockerHost, dockerPort, cancellationToken);
                await container.ExpectTextAsync("Connecting to", cancellationToken);
                await game.EnsureStableAsync(cancellationToken);
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

            await LogAsync($"Logs are stable after {Stopwatch.GetElapsedTime(timestamp).TotalSeconds:F2} seconds", cancellationToken);
            await MakeStepAsync("stable", cancellationToken);
        }

        private async Task StartVanillaAsync(string dockerHost, int dockerPort, CancellationToken cancellationToken = default)
        {
            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new("version", ProtocolVersion.FirstRelease),
                new("argument", "--fetch-exclude-all"),
                new("argument", "--username"),
                new("argument", Username),
                new("argument", "--join-server"),
                new("argument", dockerHost),
                new("argument", "--join-server-port"),
                new("argument", dockerPort.ToString()),
                new("argument", "--jvm-arg=-Djava.awt.headless=false")
            };

            var requestUri = CreateRequestUri("/start-vanilla", queryParameters);

            while (true)
            {
                using var response = await HttpClient.GetAsync(requestUri, cancellationToken);

                if (response.IsSuccessStatusCode)
                    return;

                var status = await TryReadApiStatusAsync(response, cancellationToken);

                if (response.StatusCode is HttpStatusCode.Conflict && status?.State is "stopping")
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(ClientStatePollDelayMilliseconds), cancellationToken);
                    continue;
                }

                await EnsureSuccessAsync(response, $"Starting Minecraft {ProtocolVersion.FirstRelease}", cancellationToken);
            }
        }

        private async Task SendChatAsync(string text, CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync(CreateRequestUri("/send-chat", [new KeyValuePair<string, string>("message", text)]), cancellationToken);
            await EnsureSuccessAsync(response, $"Sending chat input: {text}", cancellationToken);
        }

        private async Task StopClientAsync(CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync("/stop-client", cancellationToken);

            if (response.StatusCode is HttpStatusCode.NotFound)
                return;

            await EnsureSuccessAsync(response, $"Stopping Minecraft {ProtocolVersion.FirstRelease}", cancellationToken);
            await WaitForClientIdleAsync(cancellationToken);
        }

        private async Task WaitForClientIdleAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                using var response = await HttpClient.GetAsync("/status", cancellationToken);

                if (response.StatusCode is HttpStatusCode.NotFound)
                    return;

                await EnsureSuccessAsync(response, "Reading Minecraft client status", cancellationToken);

                var status = await ReadApiStatusAsync(response, "Reading Minecraft client status", cancellationToken);

                if (status.State is "idle")
                    return;

                if (status.State is "failed")
                    throw new IntegrationTestException($"Stopping Minecraft {ProtocolVersion.FirstRelease} failed: {status.Error ?? "client entered failed state"}");

                await Task.Delay(TimeSpan.FromMilliseconds(ClientStatePollDelayMilliseconds), cancellationToken);
            }
        }

        private async Task<byte[]> TakeScreenshotAsync(CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync("/screen", cancellationToken);
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

        private static string CreateRequestUri(string path, IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            var query = string.Join("&", queryParameters.Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));
            return $"{path}?{query}";
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

        private record ApiStatus(string Status, string State, int OperationId, string? Operation, int? Pid, string? Message, string? Error, DateTimeOffset UpdatedAt);
    }
}
