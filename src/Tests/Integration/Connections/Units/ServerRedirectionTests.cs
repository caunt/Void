namespace Void.Tests.Integration.Connections.Units;

using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

public class ServerRedirectionTests(ServerRedirectionTests.MultiServerFixture fixture) : ConnectionUnitBase, IClassFixture<ServerRedirectionTests.MultiServerFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerSwitchesBetweenServers()
    {
        var toServer2 = $"to server2 test #{Random.Shared.Next()}";
        var toServer1 = $"to server1 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessagesAsync(
                $"localhost:{ProxyPort}",
                ProtocolVersion.MINECRAFT_1_20_3,
                new[] { "/server args-server-2", toServer2, "/server args-server-1", toServer1 },
                cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync(toServer2, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(toServer1, lookupHistory: true, cancellationTokenSource.Token);
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class MultiServerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public MultiServerFixture() : base(nameof(ServerRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "server1", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "server2", cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServer: $"localhost:{Server1Port}", proxyPort: ProxyPort, additionalServers: new[] { $"localhost:{Server2Port}" }, cancellationToken: cancellationTokenSource.Token);
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
