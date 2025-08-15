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

public class ServerRedirectionTests(ServerRedirectionTests.ProxiedFixture fixture) : ConnectionUnitBase, IClassFixture<ServerRedirectionTests.ProxiedFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";

    [ProxiedFact]
    public async Task MineflayerMovesBetweenServers()
    {
        var message1 = $"server1 message #{Random.Shared.Next()}";
        var message2 = $"server2 message #{Random.Shared.Next()}";
        var message3 = $"server1 again #{Random.Shared.Next()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SwitchServersAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, Server1Name, Server2Name, message1, message2, message3, cancellationTokenSource.Token);

            await fixture.PaperServer1.ExpectTextAsync(message1, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(message2, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(message3, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(message1));
            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(message2));
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(message3));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class ProxiedFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public ProxiedFixture() : base(nameof(ServerRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, plugins: PaperPlugins.None, instanceName: "server1", version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, plugins: PaperPlugins.None, instanceName: "server2", version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServers: [$"localhost:{Server1Port}", $"localhost:{Server2Port}"], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

