using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections;

public class ProxiedConnectionTests(ProxiedConnectionTests.PaperVoidMccFixture fixture) : IClassFixture<ProxiedConnectionTests.PaperVoidMccFixture>
{
    private const string ExpectedText = "hello proxied void!";

    [Fact]
    public async Task MccConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        await fixture.Client.SendTextMessageAsync("localhost:25566", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
        await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

        Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
    }

    [Theory]
    [MemberData(nameof(MinecraftConsoleClient.SupportedVersions), MemberType = typeof(MinecraftConsoleClient))]
    public async Task MccConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        await fixture.Client.SendTextMessageAsync("localhost:25566", protocolVersion, expectedText, cancellationTokenSource.Token);
        await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

        Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
    }

    public class PaperVoidMccFixture : ConnectionTestBase, IAsyncLifetime
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

            Server = await PaperServer.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            Proxy = await VoidProxy.CreateAsync(targetServer: "localhost:25565", proxyPort: 25566, cancellationToken: cancellationTokenSource.Token);
            Client = await MinecraftConsoleClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            await Client.DisposeAsync();
            await Server.DisposeAsync();
        }
    }
}
