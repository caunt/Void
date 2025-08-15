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

public class ProxiedServerSwitchTests(ProxiedServerSwitchTests.PaperVoidTwoPaperFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerSwitchTests.PaperVoidTwoPaperFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string JoinText = "joined the game";

    [ProxiedFact]
    public async Task MineflayerMovesBetweenServersThroughProxy()
    {
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendMessagesAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_21_7, ["/server args-server-2", "/server args-server-1"], cancellationTokenSource.Token);

            var server1Joins = fixture.PaperServer1.Logs.Count(line => line.Contains(JoinText, StringComparison.OrdinalIgnoreCase));
            var server2Joins = fixture.PaperServer2.Logs.Count(line => line.Contains(JoinText, StringComparison.OrdinalIgnoreCase));

            Assert.True(server2Joins >= 1, "Expected to join second server at least once.");
            Assert.True(server1Joins >= 2, "Expected to return to first server.");
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class PaperVoidTwoPaperFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public PaperVoidTwoPaperFixture() : base(nameof(ProxiedServerSwitchTests))
        {
        }

        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, plugins: PaperPlugins.None, name: "ServerOne", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, plugins: PaperPlugins.None, name: "ServerTwo", cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServers: new[] { $"localhost:{Server1Port}", $"localhost:{Server2Port}" }, proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

