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
            Assert.True(fixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is >= 2); // TODO: sometimes, proxy prints multiple times "connected to" message
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class Fixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public Fixture() : base(nameof(ProxiedServerRedirectionTests))
        {
        }

        private MineflayerClient? _mineflayerClient;
        private PaperServer? _paperServer1;
        private PaperServer? _paperServer2;
        private VoidProxy? _voidProxy;

        public MineflayerClient MineflayerClient => _mineflayerClient ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized.");
        public PaperServer PaperServer1 => _paperServer1 ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized.");
        public PaperServer PaperServer2 => _paperServer2 ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized.");
        public VoidProxy VoidProxy => _voidProxy ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized.");

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            var mineflayerClientTask = MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            var paperServer1Task = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "server1", cancellationToken: cancellationTokenSource.Token);
            var paperServer2Task = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "server2", cancellationToken: cancellationTokenSource.Token);
            var voidProxyTask = VoidProxy.CreateAsync([$"localhost:{Server1Port}", $"localhost:{Server2Port}"], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);

            _mineflayerClient = await mineflayerClientTask;
            _paperServer1 = await paperServer1Task;
            _paperServer2 = await paperServer2Task;
            _voidProxy = await voidProxyTask;
        }

        public async Task DisposeAsync()
        {
            if (_mineflayerClient is not null)
                await _mineflayerClient.DisposeAsync();

            if (_paperServer1 is not null)
                await _paperServer1.DisposeAsync();

            if (_paperServer2 is not null)
                await _paperServer2.DisposeAsync();

            if (_voidProxy is not null)
                await _voidProxy.DisposeAsync();
        }
    }
}

