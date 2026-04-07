using System;
using System.Collections.Generic;
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
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PortableMinecraftClient(IContainer Container) : IIntegrationSide
{
    private const string Display = ":99";
    private const string RedirectOutput = ">/proc/1/fd/1 2>/proc/1/fd/2";

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } =
    [
        .. ProtocolVersion
        .Range(ProtocolVersion.Oldest, ProtocolVersion.Latest)
        // These do connect to the server before the loading screen completes, causing MC-228828 crash on 1.14-1.19
        .Where(version => version < ProtocolVersion.MINECRAFT_1_14 || version > ProtocolVersion.MINECRAFT_1_19)
    ];

    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => Container.ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

    public static async Task<PortableMinecraftClient> CreateAsync(PortableMinecraftClientImageFixture clientImageFixture, CancellationToken cancellationToken = default)
    {
        var builder = new ContainerBuilder(image: "rust:bookworm")
            .WithImage(clientImageFixture.DockerImage)
            .WithEnvironment("DISPLAY", Display);

        if (OperatingSystem.IsLinux())
            builder = builder.WithCreateParameterModifier(parameters => parameters.HostConfig.NetworkMode = "host");

        var container = builder.Build();

        await container.StartAsync(cancellationToken);
        await container.RunCommandAsync(["start-display"], cancellationToken);

        return new PortableMinecraftClient(container);
    }

    public async Task<Memory<byte>> SendTextMessageAsync(string text, CancellationToken cancellationToken = default)
    {
        return await SendTextMessagesAsync([text], cancellationToken);
    }

    public async Task<Memory<byte>> SendTextMessagesAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        foreach (var text in texts)
        {
            // Is it a Chat Command?
            var expectTask = text.StartsWith('/')
                ? Task.Delay(3_000, cancellationToken) // Give some room delay for client-server reaction
                : Container.ExpectTextAsync(text, cancellationToken); // Expect the text to appear in logs

            await Container.RunCommandAsync(["send-chat", text], cancellationToken);
            await expectTask;
        }

        return await Container.TakeScreenshotAsync(Display, cancellationToken);
    }

    public async Task EnsureStableAsync(CancellationToken cancellationToken = default)
    {
        await EnsureStableAsync(TimeSpan.FromSeconds(10), cancellationToken);
    }

    public async Task EnsureStableAsync(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        // Ensure the game is stable and not doing some background loading
        await Container.WaitForLogsSilenceAsync(duration, whitelist:
        [
            // These are spam messages that can appear because of poor ViaVersion protocol support
            "Unable to play unknown soundEvent", // minecraft:mob.rabbit.hop (or .idle) for example - bunnies often do hop nearby
            " has no item?!", // Item entity 72 has no item?!
            "Skipping Entity with id" // Skipping Entity with id minecraft:cave_spider
        ], cancellationToken);
    }

    public async Task<IAsyncDisposable> RunGameAsync(EndPoint endPoint, ProtocolVersion protocolVersion, CancellationToken cancellationToken = default)
    {
        await Container.RunCommandAsync($"echo \"Starting Minecraft {protocolVersion.FirstRelease}\" {RedirectOutput}", cancellationToken);

        // Reset whole options.txt (since it is not backwards-compatible, it should reset each time this method executes)
        await Container.RunCommandAsync($"printf \"onboardAccessibility:false\npauseOnLostFocus:false\" > \"$HOME/.minecraft/options.txt\"", cancellationToken);

        var (dockerHost, dockerPort) = endPoint.AsDockerHostPort;

        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var task = Container.RunCommandAsync($"portablemc start --username \"{nameof(PortableMinecraftClient)[..16]}\" --join-server \"{dockerHost}\" --join-server-port \"{dockerPort}\" --jvm-arg=-Djava.awt.headless=false \"{protocolVersion.FirstRelease}\" {RedirectOutput}", cancellationToken, cancellationTokenSource.Token);

        await Container.ExpectTextAsync("Connecting to", cancellationToken);
        await EnsureStableAsync(cancellationToken);

        return AsyncDisposable.Create(async () =>
        {
            await cancellationTokenSource.CancelAsync();

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        });
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
}
