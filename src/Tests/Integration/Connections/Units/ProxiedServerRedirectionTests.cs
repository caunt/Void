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

public class ProxiedServerRedirectionTests(ProxiedServerRedirectionTests.MultiServerMineflayerFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerRedirectionTests.MultiServerMineflayerFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerMovesBetweenServers()
    {
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            var commands = new[] { "/server args-server-2", "/server args-server-1" };
            await fixture.MineflayerClient.SendCommandsAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, commands, cancellationTokenSource.Token);

            Assert.Contains(fixture.VoidProxy.Logs, line => line.Contains("Player MineflayerClient connected to args-server-2"));
            var server1Connections = fixture.VoidProxy.Logs.Count(line => line.Contains("Player MineflayerClient connected to args-server-1"));

            Assert.True(server1Connections >= 2, "Client did not return to server1");
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class MultiServerMineflayerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public MultiServerMineflayerFixture() : base(nameof(ProxiedServerRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "Server1", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "Server2", cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(new[] { $"localhost:{Server1Port}", $"localhost:{Server2Port}" }, proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
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
