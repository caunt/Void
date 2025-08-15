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

public class ProxiedRedirectionTests(ProxiedRedirectionTests.VoidSetup fixture) : ConnectionUnitBase, IClassFixture<ProxiedRedirectionTests.VoidSetup>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task Mineflayer_Redirects_Between_Servers()
    {
        var message1 = $"message1 {Random.Shared.Next()}";
        var message2 = $"message2 {Random.Shared.Next()}";
        var message3 = $"message3 {Random.Shared.Next()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendCommandsAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, new[]
            {
                message1,
                "/server args-server-2",
                message2,
                "/server args-server-1",
                message3
            }, cancellationTokenSource.Token);

            await fixture.PaperServer1.ExpectTextAsync(message1, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(message2, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(message3, lookupHistory: true, cancellationTokenSource.Token);
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class VoidSetup : ConnectionFixtureBase, IAsyncLifetime
    {
        public VoidSetup() : base(nameof(ProxiedRedirectionTests))
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
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: "server1", version: "1.20.4", port: Server1Port, cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: "server2", version: "1.20.4", port: Server2Port, cancellationToken: cancellationTokenSource.Token);

            VoidProxy = await VoidProxy.CreateAsync(new[]
            {
                $"localhost:{Server1Port}",
                $"localhost:{Server2Port}"
            }, proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

