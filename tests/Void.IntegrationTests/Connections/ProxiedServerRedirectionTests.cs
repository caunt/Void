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
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    private static readonly EndPoint ProxyEndPoint = new IPEndPoint(IPAddress.Loopback, ProxyPort);

    [Fact]
    public async Task MineflayerMovesBetweenPaperServersThroughProxy()
    {
        var server1First = $"server1-{Guid.NewGuid()}";
        var server2Text = $"server2-{Guid.NewGuid()}";

        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessagesAsync(
                ProxyEndPoint,
                ProtocolVersion.MINECRAFT_1_21_6,
                [
                    server1First,
                    "/server args-server-2",
                    server2Text,
                    "/server args-server-1"
                ],
                cancellationTokenSource.Token);

            Assert.Contains(fixture.VoidProxy.Logs, line => line.Contains("connected to args-server-2"));
            Assert.True(fixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is >= 2); // TODO: sometimes, proxy prints multiple times "connected to" message
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [Theory]
    [MemberData(nameof(PortableMinecraftClient.SupportedVersions), MemberType = typeof(PortableMinecraftClient))]
    public async Task MineflayerMovesBetweenPaperServersThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var server1First = $"server1-{Guid.NewGuid()}";
        var server2Text = $"server2-{Guid.NewGuid()}";

        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessagesAsync(
                ProxyEndPoint,
                protocolVersion,
                [
                    server1First,
                    "/server args-server-2",
                    server2Text,
                    "/server args-server-1"
                ],
                cancellationTokenSource.Token);

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

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(_workingDirectory, cancellationTokenSource.Token);
            var paperServer1Task = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, name: "server1", cancellationToken: cancellationTokenSource.Token);
            var paperServer2Task = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, name: "server2", cancellationToken: cancellationTokenSource.Token);
            var voidProxyTask = VoidProxy.CreateAsync(_workingDirectory, [$"localhost:{Server1Port}", $"localhost:{Server2Port}"], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);

            PortableMinecraftClient = await portableMinecraftClientTask;
            PaperServer1 = await paperServer1Task;
            PaperServer2 = await paperServer2Task;
            VoidProxy = await voidProxyTask;
        }

        public async Task DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer1.DisposeAsync();
            await PaperServer2.DisposeAsync();
            await VoidProxy.DisposeAsync();
        }
    }
}

