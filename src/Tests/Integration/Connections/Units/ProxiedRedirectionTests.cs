using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedRedirectionTests(ProxiedRedirectionTests.TwoServersFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedRedirectionTests.TwoServersFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerSwitchesBetweenServers()
    {
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendCommandsAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_21_4, ["/server args-server-2", "/server args-server-1"], cancellationTokenSource.Token);

            Assert.Contains(fixture.Server2.Logs, line => line.Contains("joined the game"));
            Assert.True(fixture.Server1.Logs.Count(line => line.Contains("joined the game")) >= 2);
        }, fixture.MineflayerClient, fixture.Proxy, fixture.Server1, fixture.Server2);
    }

    public class TwoServersFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public TwoServersFixture() : base(nameof(ProxiedRedirectionTests))
        {
        }

        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer Server1 { get => field ?? throw new InvalidOperationException($"{nameof(Server1)} is not initialized."); set; }
        public PaperServer Server2 { get => field ?? throw new InvalidOperationException($"{nameof(Server2)} is not initialized."); set; }
        public VoidProxy Proxy { get => field ?? throw new InvalidOperationException($"{nameof(Proxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            Server1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, "server1", port: Server1Port, cancellationToken: cancellationTokenSource.Token);
            Server2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, "server2", port: Server2Port, cancellationToken: cancellationTokenSource.Token);
            Proxy = await VoidProxy.CreateAsync([
                    $"localhost:{Server1Port}",
                    $"localhost:{Server2Port}"
                ], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (MineflayerClient is not null)
                await MineflayerClient.DisposeAsync();

            if (Server1 is not null)
                await Server1.DisposeAsync();

            if (Server2 is not null)
                await Server2.DisposeAsync();

            if (Proxy is not null)
                await Proxy.DisposeAsync();
        }
    }
}

