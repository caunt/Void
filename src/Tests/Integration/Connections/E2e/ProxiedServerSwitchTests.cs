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

namespace Void.Tests.Integration.Connections.E2e;

public class ProxiedServerSwitchTests(ProxiedServerSwitchTests.ProxyFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerSwitchTests.ProxyFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";

    [ProxiedFact]
    public async Task MineflayerSwitchesServersThroughProxy()
    {
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SwitchServersAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, Server2Name, Server1Name, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains("joined the game"));
            Assert.True(fixture.PaperServer1.Logs.Count(line => line.Contains("joined the game")) >= 2);
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class ProxyFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public ProxyFixture() : base(nameof(ProxiedServerSwitchTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, plugins: PaperPlugins.None, version: "1.20.4", cancellationToken: cancellationTokenSource.Token, instanceName: "PaperServer1");
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, plugins: PaperPlugins.None, version: "1.20.4", cancellationToken: cancellationTokenSource.Token, instanceName: "PaperServer2");
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

