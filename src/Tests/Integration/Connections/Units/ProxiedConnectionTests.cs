using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections.Units;

public class ProxiedConnectionTests(ProxiedConnectionTests.PaperVoidMccFixture fixture) : ConnectionUnitBase, IClassFixture<ProxiedConnectionTests.PaperVoidMccFixture>
{
    private const int ProxyPort = 35000;
    private const int ServerPort = 35001;
    private const string ExpectedText = "hello proxied void!";

    [Fact]
    public async Task MccConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await LoggedExecutorAsync(async () =>
        {
            await fixture.Client.SendTextMessageAsync($"localhost:{ProxyPort}", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
        }, fixture.Client, fixture.Proxy, fixture.Server);
    }

    [Theory]
    [MemberData(nameof(MinecraftConsoleClient.SupportedVersions), MemberType = typeof(MinecraftConsoleClient))]
    public async Task MccConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await LoggedExecutorAsync(async () =>
        {
            await fixture.Client.SendTextMessageAsync($"localhost:{ProxyPort}", protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
        }, fixture.Client, fixture.Proxy, fixture.Server);
    }

    public class PaperVoidMccFixture : ConnectionFixtureBase, IAsyncLifetime
    {
        public PaperVoidMccFixture() : base(nameof(ProxiedConnectionTests))
        {
        }

        public PaperServer Server { get; private set; } = null!;
        public VoidProxy Proxy { get; private set; } = null!;
        public MinecraftConsoleClient Client { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));

            Server = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);
            Proxy = await VoidProxy.CreateAsync(targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
            Client = await MinecraftConsoleClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (Client is not null)
                await Client.DisposeAsync();

            if (Proxy is not null)
                await Proxy.DisposeAsync();

            if (Server is not null)
                await Server.DisposeAsync();
        }
    }
}
