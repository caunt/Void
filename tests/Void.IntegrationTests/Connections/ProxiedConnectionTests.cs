using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class ProxiedConnectionTests(ProxiedConnectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<ProxiedConnectionTests.Fixture>
{
    private const string ExpectedText = "hello proxied void!";

    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, fixture.VoidProxy.Port);

    [Fact]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy()
    {
        await PortableMinecraftClientConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion.MINECRAFT_1_20_3);
    }

    [Theory]
    [MemberData(nameof(PortableMinecraftClient.SupportedVersions), MemberType = typeof(PortableMinecraftClient))]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(StepTimeout * 3)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await fixture.PortableMinecraftClient.RunGameAsync(_proxyEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await fixture.PortableMinecraftClient.SendTextMessageAsync(expectedText, StepTimeoutToken);
                await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, StepTimeoutToken);
            }

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class Fixture() : IntegrationFixtureBase(nameof(ProxiedConnectionTests)), IAsyncLifetime
    {
        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async ValueTask InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(cancellationTokenSource.Token);
            var paperServerTask = PaperServer.CreateAsync(cancellationTokenSource.Token);

            PaperServer = await paperServerTask;
            var voidProxyTask = VoidProxy.CreateAsync(_workingDirectory, targetServer: $"localhost:{PaperServer.Port}", cancellationToken: cancellationTokenSource.Token);

            PortableMinecraftClient = await portableMinecraftClientTask;
            VoidProxy = await voidProxyTask;
        }

        public async ValueTask DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer.DisposeAsync();
            await VoidProxy.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
