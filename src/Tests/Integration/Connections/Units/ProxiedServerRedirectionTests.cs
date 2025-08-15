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

public class ProxiedServerRedirectionTests(ProxiedServerRedirectionTests.SwitchServersFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedServerRedirectionTests.SwitchServersFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";

    [ProxiedFact]
    public async Task MineflayerSwapsServersUsingServerCommand()
    {
        var messageToServer2 = $"server2 {Random.Shared.Next()}";
        var messageToServer1 = $"server1 {Random.Shared.Next()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessagesAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, [
                $"/server {Server2Name}",
                messageToServer2,
                $"/server {Server1Name}",
                messageToServer1
            ], cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync(messageToServer2, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(messageToServer1, lookupHistory: true, cancellationTokenSource.Token);
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class SwitchServersFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public SwitchServersFixture() : base(nameof(ProxiedServerRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, name: "server1", port: Server1Port, cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, name: "server2", port: Server2Port, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync([
                $"localhost:{Server1Port}",
                $"localhost:{Server2Port}"
            ], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

