using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Sides.Clients;
using Void.IntegrationTests.Infrastructure.Sides.Servers;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class DirectConnectionTests(DirectConnectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<DirectConnectionTests.Fixture>
{
    private const int ServerPort = 25000;
    private const string ExpectedText = "hello void!";

    private static readonly EndPoint ServerEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);

    [Fact]
    public async Task PortableMinecraftClientConnectsToPaperServer()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessageAsync(ServerEndPoint, ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.PaperServer);
    }

    [Theory]
    [MemberData(nameof(MinecraftConsoleClient.SupportedVersions), MemberType = typeof(MinecraftConsoleClient))]
    public async Task PortableMinecraftClientConnectsToPaperServer_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessageAsync(ServerEndPoint, protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.PaperServer);
    }

    public class Fixture() : IntegrationFixtureBase(nameof(DirectConnectionTests)), IAsyncLifetime
    {
        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(_workingDirectory, cancellationTokenSource.Token);
            var paperServerTask = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);

            PortableMinecraftClient = await portableMinecraftClientTask;
            PaperServer = await paperServerTask;
        }

        public async Task DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer.DisposeAsync();
        }
    }
}
