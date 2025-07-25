using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedConnectionTests(ProxiedConnectionTests.PaperVoidMccFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedConnectionTests.PaperVoidMccFixture>
{
    private const int ProxyPort = 35000;
    private const int ServerPort = 35001;
    private const string ExpectedText = "hello proxied void!";

    [Fact]
    public async Task MccConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer);
    }

    [Theory]
    [MemberData(nameof(MinecraftConsoleClient.SupportedVersions), MemberType = typeof(MinecraftConsoleClient))]
    public async Task MccConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer);
    }

    [Fact]
    public async Task MineflayerConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer);
    }

    [Theory]
    [MemberData(nameof(MineflayerClient.SupportedVersions), MemberType = typeof(MineflayerClient))]
    public async Task MineflayerConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class PaperVoidMccFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public PaperVoidMccFixture() : base(nameof(ProxiedConnectionTests))
        {
        }

        public PaperServer PaperServer { get; private set; } = null!;
        public VoidProxy VoidProxy { get; private set; } = null!;
        public MinecraftConsoleClient MinecraftConsoleClient { get; private set; } = null!;
        public MineflayerClient MineflayerClient { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MinecraftConsoleClient = await MinecraftConsoleClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            PaperServer = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (MinecraftConsoleClient is not null)
                await MinecraftConsoleClient.DisposeAsync();

            if (MineflayerClient is not null)
                await MineflayerClient.DisposeAsync();

            if (PaperServer is not null)
                await PaperServer.DisposeAsync();

            if (VoidProxy is not null)
                await VoidProxy.DisposeAsync();
        }
    }
}
