using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedServerSwitchTests(ProxiedServerSwitchTests.TwoPaperVoidMineflayerFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerSwitchTests.TwoPaperVoidMineflayerFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerSwitchesBetweenPaperServers()
    {
        const string Message1 = "hello from server1";
        const string Message2 = "hello from server2";
        const string Message3 = "hello again server1";
        const string Server1Name = "args-server-1";
        const string Server2Name = "args-server-2";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SwitchServersAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, Server1Name, Server2Name, Message1, Message2, Message3, cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync(Message2, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(Message3, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(Message2));
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(Message3));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class TwoPaperVoidMineflayerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public TwoPaperVoidMineflayerFixture() : base(nameof(ProxiedServerSwitchTests))
        {
        }

        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: "PaperServer1", port: Server1Port, plugins: PaperPlugins.None, version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: "PaperServer2", port: Server2Port, plugins: PaperPlugins.None, version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServer: $"localhost:{Server1Port}", proxyPort: ProxyPort, additionalServers: [$"localhost:{Server2Port}"], cancellationToken: cancellationTokenSource.Token);
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

