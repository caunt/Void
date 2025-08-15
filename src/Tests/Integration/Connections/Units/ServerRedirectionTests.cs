using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;
using Void.Tests.Integration;
using Void.Tests.Integration.Connections;

namespace Void.Tests.Integration.Connections.Units;

public class ServerRedirectionTests(ServerRedirectionTests.RedirectionFixture fixture) : ConnectionUnitBase, IClassFixture<ServerRedirectionTests.RedirectionFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string Server1Name = "args-server-1";
    private const string Server2Name = "args-server-2";
    private const string ExpectedText = "hello redirection";

    [ProxiedFact]
    public async Task MineflayerMovesBetweenServers()
    {
        var firstMessage = $"{ExpectedText} #1 #{Random.Shared.Next()}";
        var secondMessage = $"{ExpectedText} #2 #{Random.Shared.Next()}";
        var thirdMessage = $"{ExpectedText} #3 #{Random.Shared.Next()}";

        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.MineflayerClient.SwitchServersAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_21_4, Server1Name, Server2Name, firstMessage, secondMessage, thirdMessage, cancellationTokenSource.Token);

            await fixture.Server1.ExpectTextAsync(firstMessage, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server2.ExpectTextAsync(secondMessage, lookupHistory: true, cancellationTokenSource.Token);
            await fixture.Server1.ExpectTextAsync(thirdMessage, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server1.Logs, line => line.Contains(firstMessage));
            Assert.Contains(fixture.Server2.Logs, line => line.Contains(secondMessage));
            Assert.Contains(fixture.Server1.Logs, line => line.Contains(thirdMessage));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.Server1, fixture.Server2);
    }

    public class RedirectionFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public RedirectionFixture() : base(nameof(ServerRedirectionTests))
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
            const string paperVersion = "1.20.4";
            Server1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, instanceName: "server1", minecraftVersion: paperVersion, cancellationToken: cancellationTokenSource.Token);
            Server2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, instanceName: "server2", minecraftVersion: paperVersion, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(new[] { $"localhost:{Server1Port}", $"localhost:{Server2Port}" }, proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

