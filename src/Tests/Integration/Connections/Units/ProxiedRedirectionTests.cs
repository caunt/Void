using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedRedirectionTests(ProxiedRedirectionTests.TwoServersFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedRedirectionTests.TwoServersFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;

    [ProxiedFact]
    public async Task MineflayerRedirectsBetweenTwoServers()
    {
        var firstMessage = $"first-{Random.Shared.Next()}";
        var secondMessage = $"second-{Random.Shared.Next()}";
        var thirdMessage = $"third-{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SendCommandsAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, new[]
            {
                firstMessage,
                "/server server2",
                secondMessage,
                "/server server1",
                thirdMessage
            }, cancellationTokenSource.Token);

            await fixture.Server1.ExpectTextAsync(firstMessage, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server2.ExpectTextAsync(secondMessage, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server1.ExpectTextAsync(thirdMessage, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server1.Logs, line => line.Contains(firstMessage));
            Assert.Contains(fixture.Server2.Logs, line => line.Contains(secondMessage));
            Assert.Contains(fixture.Server1.Logs, line => line.Contains(thirdMessage));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.Server1, fixture.Server2);
    }

    public class TwoServersFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        private const string Server1Name = "server1";
        private const string Server2Name = "server2";

        public TwoServersFixture() : base(nameof(ProxiedRedirectionTests))
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
            Server1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: Server1Name, port: Server1Port, cancellationToken: cancellationTokenSource.Token);
            Server2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, instanceName: Server2Name, port: Server2Port, cancellationToken: cancellationTokenSource.Token);

            VoidProxy = await VoidProxy.CreateAsync(targetServer: $"{Server1Name}=localhost:{Server1Port}", proxyPort: ProxyPort, additionalServers: [$"{Server2Name}=localhost:{Server2Port}"], cancellationToken: cancellationTokenSource.Token);
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
