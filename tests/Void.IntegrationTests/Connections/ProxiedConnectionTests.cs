using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class ProxiedConnectionTests(ProxiedConnectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<ProxiedConnectionTests.Fixture>
{
    private const int ProxyPort = 35000;
    private const string ExpectedText = "hello proxied void!";

    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, fixture.PaperServer.Port);

    [Fact]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessageAsync(_proxyEndPoint, ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    [Theory]
    [MemberData(nameof(PortableMinecraftClient.SupportedVersions), MemberType = typeof(PortableMinecraftClient))]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessageAsync(_proxyEndPoint, protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class Fixture() : IntegrationFixtureBase(nameof(ProxiedConnectionTests)), IAsyncLifetime
    {
        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async ValueTask InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(cancellationTokenSource.Token);
            var paperServerTask = PaperServer.CreateAsync(cancellationTokenSource.Token);

            PortableMinecraftClient = await portableMinecraftClientTask;
            PaperServer = await paperServerTask;
            VoidProxy = await VoidProxy.CreateAsync(_workingDirectory, targetServer: $"localhost:{PaperServer.Port}", proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
        }

        public async ValueTask DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer.DisposeAsync();
            await VoidProxy.DisposeAsync();
        }
    }
}
