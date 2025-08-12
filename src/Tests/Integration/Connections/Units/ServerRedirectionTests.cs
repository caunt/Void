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

public class ServerRedirectionTests(ServerRedirectionTests.MultiServerFixture fixture) : ConnectionUnitBase, IClassFixture<ServerRedirectionTests.MultiServerFixture>
{
    private const int ProxyPort = 36000;
    private const int Server1Port = 36001;
    private const int Server2Port = 36002;
    private const string ExpectedText = "hello multi-server void!";

    // Override timeout for multi-server tests as they take longer
    public new TimeSpan Timeout { get; } = TimeSpan.FromMinutes(5);

    [ProxiedFact]
    public async Task BasicMultiServerSetup_ConnectsToFirstServer()
    {
        var testMessage = $"{ExpectedText} basic setup test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            // Connect to proxy (should route to first server by default)
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage, cancellationTokenSource.Token);
            
            // Verify client is connected to server1 (first server in the list)
            await fixture.PaperServer1.ExpectTextAsync(testMessage, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage));

        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [ProxiedFact]
    public async Task MccRedirectsBetweenServers()
    {
        var testMessage1 = $"{ExpectedText} server1 test #{Random.Shared.Next()}";
        var testMessage2 = $"{ExpectedText} server2 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            // Connect to proxy (should route to first server by default)
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage1, cancellationTokenSource.Token);
            
            // Verify client is connected to server1
            await fixture.PaperServer1.ExpectTextAsync(testMessage1, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage1));

            // Redirect to server2 using /server command
            await fixture.MinecraftConsoleClient.SendCommandAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, "/server args-server-2", TimeSpan.FromSeconds(10), cancellationTokenSource.Token);
            
            // Send message to verify we're on server2
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage2, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(testMessage2, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(testMessage2));

            // Redirect back to server1 using /server command  
            await fixture.MinecraftConsoleClient.SendCommandAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, "/server args-server-1", TimeSpan.FromSeconds(10), cancellationTokenSource.Token);
            
            // Send message to verify we're back on server1
            var finalMessage = $"{ExpectedText} back to server1 test #{Random.Shared.Next()}";
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, finalMessage, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(finalMessage, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(finalMessage));

        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [ProxiedFact]
    public async Task MineflayerRedirectsBetweenServers()
    {
        var testMessage1 = $"{ExpectedText} mineflayer server1 test #{Random.Shared.Next()}";
        var testMessage2 = $"{ExpectedText} mineflayer server2 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            // Connect to proxy (should route to first server by default)
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage1, cancellationTokenSource.Token);
            
            // Verify client is connected to server1
            await fixture.PaperServer1.ExpectTextAsync(testMessage1, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage1));

            // Redirect to server2 using /server command
            await fixture.MineflayerClient.SendCommandAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, "/server args-server-2", TimeSpan.FromSeconds(8), cancellationTokenSource.Token);
            
            // Send message to verify we're on server2
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage2, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(testMessage2, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(testMessage2));

            // Redirect back to server1 using /server command  
            await fixture.MineflayerClient.SendCommandAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, "/server args-server-1", TimeSpan.FromSeconds(8), cancellationTokenSource.Token);
            
            // Send message to verify we're back on server1
            var finalMessage = $"{ExpectedText} mineflayer back to server1 test #{Random.Shared.Next()}";
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, finalMessage, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(finalMessage, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(finalMessage));

        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [ProxiedTheory]
    [MemberData(nameof(MinecraftConsoleClient.SupportedVersions), MemberType = typeof(MinecraftConsoleClient))]
    public async Task MccRedirectsBetweenServers_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var testMessage1 = $"{ExpectedText} protocol {protocolVersion} server1 test #{Random.Shared.Next()}";
        var testMessage2 = $"{ExpectedText} protocol {protocolVersion} server2 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            // Connect to proxy and send initial message to server1
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, testMessage1, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(testMessage1, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage1));

            // Redirect to server2 and verify
            await fixture.MinecraftConsoleClient.SendCommandAsync($"localhost:{ProxyPort}", protocolVersion, "/server args-server-2", TimeSpan.FromSeconds(10), cancellationTokenSource.Token);
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, testMessage2, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(testMessage2, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(testMessage2));

        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [ProxiedTheory]
    [MemberData(nameof(MineflayerClient.SupportedVersions), MemberType = typeof(MineflayerClient))]
    public async Task MineflayerRedirectsBetweenServers_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var testMessage1 = $"{ExpectedText} mineflayer protocol {protocolVersion} server1 test #{Random.Shared.Next()}";
        var testMessage2 = $"{ExpectedText} mineflayer protocol {protocolVersion} server2 test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            // Connect to proxy and send initial message to server1
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, testMessage1, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(testMessage1, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage1));

            // Redirect to server2 and verify
            await fixture.MineflayerClient.SendCommandAsync($"localhost:{ProxyPort}", protocolVersion, "/server args-server-2", TimeSpan.FromSeconds(8), cancellationTokenSource.Token);
            await fixture.MineflayerClient.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, testMessage2, cancellationTokenSource.Token);
            await fixture.PaperServer2.ExpectTextAsync(testMessage2, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer2.Logs, line => line.Contains(testMessage2));

        }, fixture.MineflayerClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    [ProxiedFact]
    public async Task ServerRedirection_InvalidServerName_StaysOnCurrentServer()
    {
        var testMessage1 = $"{ExpectedText} invalid server test #{Random.Shared.Next()}";
        var testMessage2 = $"{ExpectedText} still on original server test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);

        await LoggedExecutorAsync(async () =>
        {
            // Connect to proxy and send initial message to server1
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage1, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(testMessage1, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage1));

            // Try to redirect to non-existent server
            await fixture.MinecraftConsoleClient.SendCommandAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, "/server nonexistent-server", TimeSpan.FromSeconds(5), cancellationTokenSource.Token);
            
            // Send message to verify we're still on server1 (redirection should have failed)
            await fixture.MinecraftConsoleClient.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, testMessage2, cancellationTokenSource.Token);
            await fixture.PaperServer1.ExpectTextAsync(testMessage2, lookupHistory: true, cancellationTokenSource.Token);
            Assert.Contains(fixture.PaperServer1.Logs, line => line.Contains(testMessage2));

        }, fixture.MinecraftConsoleClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }

    public class MultiServerFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public MultiServerFixture() : base(nameof(ServerRedirectionTests))
        {
        }

        public MinecraftConsoleClient MinecraftConsoleClient { get => field ?? throw new InvalidOperationException($"{nameof(MinecraftConsoleClient)} is not initialized."); set; }
        public MineflayerClient MineflayerClient { get => field ?? throw new InvalidOperationException($"{nameof(MineflayerClient)} is not initialized."); set; }
        public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
        public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);

            MinecraftConsoleClient = await MinecraftConsoleClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            MineflayerClient = await MineflayerClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            
            // Create two separate PaperServers with different names and ports
            PaperServer1 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server1Port, name: "PaperServer1", cancellationToken: cancellationTokenSource.Token);
            PaperServer2 = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: Server2Port, name: "PaperServer2", cancellationToken: cancellationTokenSource.Token);
            
            // Create proxy with both servers
            var servers = new[] { $"localhost:{Server1Port}", $"localhost:{Server2Port}" };
            VoidProxy = await VoidProxy.CreateAsync(servers, ProxyPort, cancellationToken: cancellationTokenSource.Token);
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

            _httpClient?.Dispose();
        }
    }
}