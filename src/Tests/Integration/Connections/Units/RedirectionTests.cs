using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class RedirectionTests(RedirectionTests.PaperVoidClientsFixture fixture) : ConnectionUnitBase, IClassFixture<RedirectionTests.PaperVoidClientsFixture>
{
    private const int ProxyPort = 45002;
    private const int FirstServerPort = 45000;
    private const int SecondServerPort = 45001;
    private const string FirstServerName = "args-server-1";
    private const string SecondServerName = "args-server-2";

    [ProxiedFact]
    public async Task MinecraftConsoleClient_RedirectsBetweenServers()
    {
        var playerName = nameof(MinecraftConsoleClient)[..16];
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            fixture.PaperServer1.ClearLogs();
            fixture.PaperServer2.ClearLogs();

            await fixture.MinecraftConsoleClient.RedirectBetweenServersAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, FirstServerName, SecondServerName, cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync($"{playerName} joined the game", lookupHistory: true, cancellationTokenSource.Token);

            Assert.Equal(2, fixture.PaperServer1.Logs.Count(line => line.Contains($"{playerName} joined the game")));
            Assert.Equal(1, fixture.PaperServer2.Logs.Count(line => line.Contains($"{playerName} joined the game")));
        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [ProxiedFact]
    public async Task MineflayerClient_RedirectsBetweenServers()
    {
        var playerName = nameof(MineflayerClient);
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            fixture.PaperServer1.ClearLogs();
            fixture.PaperServer2.ClearLogs();

            await fixture.MineflayerClient.RedirectBetweenServersAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, FirstServerName, SecondServerName, cancellationTokenSource.Token);

            await fixture.PaperServer2.ExpectTextAsync($"{playerName} joined the game", lookupHistory: true, cancellationTokenSource.Token);

            Assert.Equal(2, fixture.PaperServer1.Logs.Count(line => line.Contains($"{playerName} joined the game")));
            Assert.Equal(1, fixture.PaperServer2.Logs.Count(line => line.Contains($"{playerName} joined the game")));
        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class PaperVoidClientsFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public PaperVoidClientsFixture() : base(nameof(RedirectionTests))
        {
        }

        public MinecraftConsoleClient MinecraftConsoleClient { get => field ?? throw new InvalidOperationException($"{nameof(MinecraftConsoleClient)} is not initialized."); set; }
        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MinecraftConsoleClient = await MinecraftConsoleClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, name: FirstServerName, port: FirstServerPort, cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, name: SecondServerName, port: SecondServerPort, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(targetServers: [ $"localhost:{FirstServerPort}", $"localhost:{SecondServerPort}" ], proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (MinecraftConsoleClient is not null)
                await MinecraftConsoleClient.DisposeAsync();

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

