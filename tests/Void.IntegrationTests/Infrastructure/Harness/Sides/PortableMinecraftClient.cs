using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Nito.Disposables;
using Void.IntegrationTests.Infrastructure.Extensions;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.Minecraft.Network;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PortableMinecraftClient(IContainer Container) : IIntegrationSide
{
    private const int SetupRetries = 5;
    private const string Display = ":99";
    private const string RedirectOutput = ">/proc/1/fd/1 2>/proc/1/fd/2";
    private const string LogMessagePrefix = $"[{nameof(Void)}.{nameof(IntegrationTests)}]";
    
    public IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion
        .Range(ProtocolVersion.Oldest, ProtocolVersion.Latest)
        // These do connect to the server before the loading screen completes, causing MC-228828 crash on 1.14-1.19
        .Where(version => version < ProtocolVersion.MINECRAFT_1_14 || version > ProtocolVersion.MINECRAFT_1_19);

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => Container.ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

    public static async Task<PortableMinecraftClient> CreateAsync(PortableMinecraftClientImageFixture clientImageFixture, CancellationToken cancellationToken = default)
    {
        var builder = new ContainerBuilder(clientImageFixture.DockerImage)
            .WithEnvironment("DISPLAY", Display);

        if (OperatingSystem.IsLinux())
            builder = builder.WithCreateParameterModifier(parameters => parameters.HostConfig.NetworkMode = "host");

        var container = builder.Build();

        await container.StartAsync(cancellationToken);
        await container.RunCommandAsync(["start-display"], cancellationToken);

        return new PortableMinecraftClient(container);
    }

    public async Task<Game> RunGameAsync(string testName, EndPoint endPoint, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= SetupRetries; attempt++)
        {
            try
            {
                return await Game.RunAsync(testName, Container, logsDateTimeGetter: () => _readLogsSince, endPoint, protocolVersion, cancellationToken);
            }
            catch (OperationCanceledException) when (attempt < SetupRetries)
            {
                // Ignored
            }
        }

        throw new TimeoutException($"Failed to setup {protocolVersion} after {SetupRetries} attempts");
    }

    public void ClearLogs()
    {
        _readLogsSince = DateTime.UtcNow;
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public record Game(string TestName, IContainer Container, Func<DateTime> LogsDateTimeGetter, ProtocolVersion ProtocolVersion, string Username, Task RunTask, CancellationTokenSource CancellationTokenSource) : IAsyncDisposable
    {
        private readonly string _workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "steps", TestName, ProtocolVersion.ToString(), Username);
        private int _step;
        
        public DateTime ReadLogsSince => LogsDateTimeGetter();
        
        public static async Task<Game> RunAsync(string testName, IContainer container, Func<DateTime> logsDateTimeGetter, EndPoint endPoint, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
        {
            var (dockerHost, dockerPort) = endPoint.AsDockerHostPort;
            var username = Convert.ToHexString(BitConverter.GetBytes(Random.Shared.Next()));
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            // Reset whole options.txt
            await container.RunCommandAsync(
                $"""
                 printf "
                 maxFps:15
                 renderDistance:2
                 {(protocolVersion >= ProtocolVersion.MINECRAFT_1_19_4 ? "onboardAccessibility:false" : string.Empty)}
                 pauseOnLostFocus:false
                 " > "$HOME/.minecraft/options.txt"
                 """,
                cancellationToken);
            
            var task = container.RunCommandAsync($"portablemc start --fetch-exclude-all --username \"{username}\" --join-server \"{dockerHost}\" --join-server-port \"{dockerPort}\" --jvm-arg=-Djava.awt.headless=false \"{protocolVersion.FirstRelease}\" {RedirectOutput}", cancellationToken, cancellationTokenSource.Token);
            var game = new Game(testName, container, logsDateTimeGetter, protocolVersion, username, task, cancellationTokenSource);
            
            await game.LogAsync($"Starting Minecraft {protocolVersion.FirstRelease}", cancellationToken);
            await container.ExpectTextAsync("Connecting to", cancellationToken);
            await game.EnsureStableAsync(cancellationToken);

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
                await Container.RunCommandAsync(["send-chat", text], cancellationToken);
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
        
        public async ValueTask DisposeAsync()
        {
            await LogAsync($"Stopping Minecraft {ProtocolVersion.FirstRelease}", Timeouts.StepTimeoutToken);
            
            await MakeStepAsync("exit", Timeouts.StepTimeoutToken);
            await File.WriteAllLinesAsync(Path.Combine(_workingDirectory, "logs.txt"), await Container.ReadLogsAsync(ReadLogsSince, Timeouts.StepTimeoutToken), Timeouts.StepTimeoutToken);
            
            await CancellationTokenSource.CancelAsync();

            try
            {
                await RunTask;
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
            finally
            {
                CancellationTokenSource.Dispose();
            }
            
            GC.SuppressFinalize(this);
        }

        private async Task LogAsync(string message, CancellationToken cancellationToken = default)
        {
            await Container.RunCommandAsync($"printf '{LogMessagePrefix} %s\n' '{message.Replace("'", "'\"'\"'")}' {RedirectOutput}", cancellationToken);
        }

        private async Task MakeStepAsync(string action, CancellationToken cancellationToken = default)
        {
            _ = Directory.CreateDirectory(_workingDirectory);
            await File.WriteAllBytesAsync(Path.Combine(_workingDirectory, $"step-{++_step}-{action}.png"), await Container.TakeScreenshotAsync(windowName: "Minecraft", cancellationToken), cancellationToken);
        }
    }
}
