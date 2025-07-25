using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class MineflayerDirectConnectionTests(MineflayerDirectConnectionTests.PaperMineflayerFixture fixture) : ConnectionUnitBase, IClassFixture<MineflayerDirectConnectionTests.PaperMineflayerFixture>
{
    private const int ServerPort = 26000;
    private const string ExpectedText = "hello void!";

    [Fact]
    public async Task MineflayerConnectsToPaperServer()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.Client.SendTextMessageAsync($"localhost:{ServerPort}", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
        }, fixture.Client, fixture.Server);
    }

    [Theory]
    [MemberData(nameof(MineflayerClient.SupportedVersions), MemberType = typeof(MineflayerClient))]
    public async Task MineflayerConnectsToPaperServer_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.Client.SendTextMessageAsync($"localhost:{ServerPort}", protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
        }, fixture.Client, fixture.Server);
    }

    public class PaperMineflayerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public PaperMineflayerFixture() : base(nameof(MineflayerDirectConnectionTests))
        {
        }

        public PaperServer Server { get; private set; } = null!;
        public MineflayerClient Client { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            Server = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);
            Client = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (Client is not null)
                await Client.DisposeAsync();

            if (Server is not null)
                await Server.DisposeAsync();
        }
    }
}
