using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedServerRedirectionTests(ProxiedServerRedirectionTests.Fixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerRedirectionTests.Fixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerMovesBetweenPaperServersThroughProxy()
    {
        var server1First = $"server1-{Guid.NewGuid()}";
        var server2Text = $"server2-{Guid.NewGuid()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessagesAsync(
                $"localhost:{ProxyPort}",
                ProtocolVersion.MINECRAFT_1_21_4,
                [
                    server1First,
                    "/server args-server-2",
                    server2Text,
                    "/server args-server-1"
                ],
                cancellationTokenSource.Token);

            Assert.Contains(fixture.VoidProxy.Logs, line => line.Contains("connected to args-server-2"));
            Assert.True(fixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is 2);
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class Fixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public Fixture() : base(nameof(ProxiedServerRedirectionTests))
        {
        }

        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            var mineflayerClientTask = MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            var paperServer1Task = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "server1", cancellationToken: cancellationTokenSource.Token);
            var paperServer2Task = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "server2", cancellationToken: cancellationTokenSource.Token);
            var voidProxyTask = VoidProxy.CreateAsync([$"localhost:{Server1Port}", $"localhost:{Server2Port}"], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);

            MineflayerClient = await mineflayerClientTask;
            PaperServer1 = await paperServer1Task;
            PaperServer2 = await paperServer2Task;
            VoidProxy = await voidProxyTask;
        }

        public async Task DisposeAsync()
        {
            if (MineflayerClient is not null)
                await MineflayerClient.DisposeAsync();

            if (PaperServer1 is not null)
                await PaperServer1.DisposeAsync();

            if (PaperServer2 is not null)
                await PaperServer2.DisposeAsync();

            if (VoidProxy is not null)
                await VoidProxy.DisposeAsync();
        }
    }
}

