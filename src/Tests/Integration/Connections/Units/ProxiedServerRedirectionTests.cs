using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration;
using Void.Tests.Integration.Connections;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedServerRedirectionTests(ProxiedServerRedirectionTests.DualPaperVoidMineflayerFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerRedirectionTests.DualPaperVoidMineflayerFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";

    [ProxiedFact]
    public async Task MineflayerRedirectionBetweenServers()
    {
        var server1InitialText = $"server1 initial {Random.Shared.Next()}";
        var server2Text = $"server2 {Random.Shared.Next()}";
        var server1ReturnText = $"server1 return {Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendMessagesAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, new[]
            {
                server1InitialText,
                $"/server {Server2Name}",
                server2Text,
                $"/server {Server1Name}",
                server1ReturnText
            }, cancellationTokenSource.Token);

            await fixture.Server1.ExpectTextAsync(server1InitialText, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server2.ExpectTextAsync(server2Text, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server1.ExpectTextAsync(server1ReturnText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server1.Logs, line => line.Contains(server1InitialText));
            Assert.Contains(fixture.Server2.Logs, line => line.Contains(server2Text));
            Assert.Contains(fixture.Server1.Logs, line => line.Contains(server1ReturnText));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.Server1, fixture.Server2);
    }

    public class DualPaperVoidMineflayerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public DualPaperVoidMineflayerFixture() : base(nameof(ProxiedServerRedirectionTests))
        {
        }

        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer Server1 { get => field ?? throw new InvalidOperationException($"{nameof(Server1)} is not initialized."); set; }
        public PaperServer Server2 { get => field ?? throw new InvalidOperationException($"{nameof(Server2)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            Server1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: "PaperServer1", port: Server1Port, paperVersion: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            Server2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: "PaperServer2", port: Server2Port, paperVersion: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServer: $"localhost:{Server1Port}", proxyPort: ProxyPort, additionalServers: new[] { $"localhost:{Server2Port}" }, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (MineflayerClient is not null)
                await MineflayerClient.DisposeAsync();

            if (Server1 is not null)
                await Server1.DisposeAsync();

            if (Server2 is not null)
                await Server2.DisposeAsync();

            if (VoidProxy is not null)
                await VoidProxy.DisposeAsync();
        }
    }
}
