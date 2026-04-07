using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class ProxiedServerRedirectionTests(ProxiedServerRedirectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<ProxiedServerRedirectionTests.Fixture>
{
    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, fixture.VoidProxy.Port);

    [Fact]
    public async Task PortableMinecraftClientMovesBetweenPaperServersThroughProxy()
    {
        await PortableMinecraftClientMovesBetweenPaperServersThroughProxy_WithProtocolVersion(ProtocolVersion.MINECRAFT_1_21_6);
    }

    [Theory]
    [MemberData(nameof(PortableMinecraftClient.SupportedVersions), MemberType = typeof(PortableMinecraftClient))]
    public async Task PortableMinecraftClientMovesBetweenPaperServersThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var server1First = $"server1-{Guid.NewGuid()}";
        var server2Text = $"server2-{Guid.NewGuid()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(StepTimeout * 5)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await fixture.PortableMinecraftClient.RunGameAsync(_proxyEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await fixture.PortableMinecraftClient.SendTextMessagesAsync(
                [
                    server1First,
                    "/server args-server-2"
                ], StepTimeoutToken);

                await fixture.PortableMinecraftClient.EnsureStableAsync(StepTimeoutToken);

                await fixture.PortableMinecraftClient.SendTextMessagesAsync(
                [
                    server2Text,
                    "/server args-server-1"
                ], StepTimeoutToken);

                await fixture.PortableMinecraftClient.EnsureStableAsync(StepTimeoutToken);
            }

            Assert.Contains(fixture.VoidProxy.Logs, line => line.Contains("connected to args-server-2"));
            Assert.True(fixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is >= 2); // TODO: sometimes, proxy prints multiple times "connected to" message
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class Fixture() : IntegrationFixtureBase(nameof(ProxiedServerRedirectionTests)), IAsyncLifetime
    {
        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async ValueTask InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(cancellationTokenSource.Token);
            var paperServer1Task = PaperServer.CreateAsync(cancellationTokenSource.Token);
            var paperServer2Task = PaperServer.CreateAsync(cancellationTokenSource.Token);

            PaperServer1 = await paperServer1Task;
            PaperServer2 = await paperServer2Task;

            var voidProxyTask = VoidProxy.CreateAsync(_workingDirectory, targetServers: [$"localhost:{PaperServer1.Port}", $"localhost:{PaperServer2.Port}"], cancellationToken: cancellationTokenSource.Token);

            PortableMinecraftClient = await portableMinecraftClientTask;
            VoidProxy = await voidProxyTask;
        }

        public async ValueTask DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer1.DisposeAsync();
            await PaperServer2.DisposeAsync();
            await VoidProxy.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
