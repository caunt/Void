using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.E2E;

public class ProxiedRedirectionTests(ProxiedRedirectionTests.TwoPaperServersFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedRedirectionTests.TwoPaperServersFixture>
{
    private const int ProxyPort = 35000;
    private const int Server1Port = 35001;
    private const int Server2Port = 35002;
    private const string Server1Instance = "server1";
    private const string Server2Instance = "server2";
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";

    [ProxiedFact]
    public async Task MineflayerRedirectsBetweenServers()
    {
        var server1Text = $"server1 test #{Random.Shared.Next()}";
        var server2Text = $"server2 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessagesAsync(
                $"localhost:{ProxyPort}",
                ProtocolVersion.MINECRAFT_1_20_3,
                new[]
                {
                    $"/server {Server2Name}",
                    server2Text,
                    $"/server {Server1Name}",
                    server1Text
                },
                cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync(server2Text, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(server1Text, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(server2Text));
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(server1Text));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class TwoPaperServersFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public TwoPaperServersFixture() : base(nameof(ProxiedRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: Server1Instance, port: Server1Port, cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: Server2Instance, port: Server2Port, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(
                targetServer: $"localhost:{Server1Port}",
                additionalServers: new[] { $"localhost:{Server2Port}" },
                proxyPort: ProxyPort,
                cancellationToken: cancellationTokenSource.Token);
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
