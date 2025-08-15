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

public class ServerRedirectionTests(ServerRedirectionTests.Fixture fixture) : ConnectionUnitBase, IClassFixture<ServerRedirectionTests.Fixture>
{
    private const int ProxyPort = 35100;
    private const int Server1Port = 35101;
    private const int Server2Port = 35102;

    [ProxiedFact]
    public async Task MineflayerSwitchesBetweenServers()
    {
        var expected1 = $"hello server1 test #{Random.Shared.Next()}";
        var expected2 = $"hello server2 test #{Random.Shared.Next()}";
        var expected3 = $"hello again server1 test #{Random.Shared.Next()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SwitchServersAsync(
                $"localhost:{ProxyPort}",
                ProtocolVersion.MINECRAFT_1_20_3,
                new[] { "args-server-1", "args-server-2", "args-server-1" },
                new[] { expected1, expected2, expected3 },
                cancellationTokenSource.Token);

            await fixture.Server1.ExpectTextAsync(expected1, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server2.ExpectTextAsync(expected2, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server1.ExpectTextAsync(expected3, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server1.Logs, line => line.Contains(expected1));
            Assert.Contains(fixture.Server2.Logs, line => line.Contains(expected2));
            Assert.Contains(fixture.Server1.Logs, line => line.Contains(expected3));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.Server1, fixture.Server2);
    }

    public class Fixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public Fixture() : base(nameof(ServerRedirectionTests))
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
            Server1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "server1", version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
            Server2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "server2", version: "1.20.4", cancellationToken: cancellationTokenSource.Token);
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

