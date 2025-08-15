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

public class ProxiedRedirectionTests(ProxiedRedirectionTests.RedirectionFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedRedirectionTests.RedirectionFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerRedirectsBetweenPaperServersThroughProxy()
    {
        var server1Message = $"server1 {Guid.NewGuid()}";
        var server2Message = $"server2 {Guid.NewGuid()}";
        var server1ReturnMessage = $"server1 back {Guid.NewGuid()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendTextMessagesAsync(
                $"localhost:{ProxyPort}",
                ProtocolVersion.MINECRAFT_1_21_4,
                cancellationTokenSource.Token,
                server1Message,
                $"/server {fixture.Server2Name}",
                server2Message,
                $"/server {fixture.Server1Name}",
                server1ReturnMessage);

            await fixture.PaperServer1.ExpectTextAsync(server1Message, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(server2Message, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(server1ReturnMessage, lookupHistory: true, cancellationTokenSource.Token);
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class RedirectionFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public RedirectionFixture() : base(nameof(ProxiedRedirectionTests))
        {
        }

        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }
        public string Server1Name { get; private set; } = "args-server-1";
        public string Server2Name { get; private set; } = "args-server-2";

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "PaperServer1", version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "PaperServer2", version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
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
