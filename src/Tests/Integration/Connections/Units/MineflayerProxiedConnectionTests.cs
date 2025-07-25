using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class MineflayerProxiedConnectionTests(MineflayerProxiedConnectionTests.PaperVoidMineflayerFixture fixture) : ConnectionUnitBase, IClassFixture<MineflayerProxiedConnectionTests.PaperVoidMineflayerFixture>
{
    private const int ProxyPort = 36000;
    private const int ServerPort = 36001;
    private const string ExpectedText = "hello void!";

    [Fact]
    public async Task MineflayerConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        var client = fixture.Client ?? throw new InvalidOperationException("Client not initialized");
        var proxy = fixture.Proxy ?? throw new InvalidOperationException("Proxy not initialized");
        var server = fixture.Server ?? throw new InvalidOperationException("Server not initialized");

        await LoggedExecutorAsync(async () =>
        {
            await client.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(server.Logs, line => line.Contains(expectedText));
        }, client, proxy, server);
    }

    [Theory]
    [MemberData(nameof(MineflayerClient.SupportedVersions), MemberType = typeof(MineflayerClient))]
    public async Task MineflayerConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        var client = fixture.Client ?? throw new InvalidOperationException("Client not initialized");
        var proxy = fixture.Proxy ?? throw new InvalidOperationException("Proxy not initialized");
        var server = fixture.Server ?? throw new InvalidOperationException("Server not initialized");

        await LoggedExecutorAsync(async () =>
        {
            await client.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, expectedText, cancellationTokenSource.Token);
            await server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(server.Logs, line => line.Contains(expectedText));
        }, client, proxy, server);
    }

    public class PaperVoidMineflayerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public PaperVoidMineflayerFixture() : base(nameof(MineflayerProxiedConnectionTests))
        {
        }

        public PaperServer? Server { get; private set; }
        public VoidProxy? Proxy { get; private set; }
        public MineflayerClient? Client { get; private set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            Server = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);
            Proxy = await VoidProxy.CreateAsync(targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
            Client = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (Client is not null)
                await Client.DisposeAsync();

            if (Proxy is not null)
                await Proxy.DisposeAsync();

            if (Server is not null)
                await Server.DisposeAsync();
        }
    }
}
