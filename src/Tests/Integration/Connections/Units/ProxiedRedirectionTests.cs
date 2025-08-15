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

public class ProxiedRedirectionTests(ProxiedRedirectionTests.MultiplePaperVoidMineflayerFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedRedirectionTests.MultiplePaperVoidMineflayerFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";

    [ProxiedFact]
    public async Task MineflayerMovesBetweenServers()
    {
        var server2Text = $"server2 test #{Random.Shared.Next()}";
        var server1Text = $"server1 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessagesAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_21_4, [
                $"/server {Server2Name}",
                server2Text,
                $"/server {Server1Name}",
                server1Text
            ], cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync(server2Text, lookupHistory: true, cancellationToken: cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(server1Text, lookupHistory: true, cancellationToken: cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(server2Text));
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(server1Text));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class MultiplePaperVoidMineflayerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public MultiplePaperVoidMineflayerFixture() : base(nameof(ProxiedRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: Server1Name, cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: Server2Name, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(new[] { $"localhost:{Server1Port}", $"localhost:{Server2Port}" }, ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

