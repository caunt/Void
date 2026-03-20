using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Base;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections;

public class ProxiedConnectionTests(ProxiedConnectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<ProxiedConnectionTests.Fixture>
{
    private const int ProxyPort = 35000;
    private const int ServerPort = 35001;
    private const string ExpectedText = "hello proxied void!";

    private static readonly EndPoint ProxyEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);

    [Fact]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessageAsync(ProxyEndPoint, ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
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
            await fixture.PortableMinecraftClient.SendTextMessageAsync(ProxyEndPoint, protocolVersion, expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class Fixture() : IntegrationFixtureBase(nameof(ProxiedConnectionTests)), IAsyncLifetime
    {
        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

            PortableMinecraftClient = await PortableMinecraftClient.CreateAsync(_workingDirectory, cancellationTokenSource.Token);
            PaperServer = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(_workingDirectory, targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer.DisposeAsync();
            await VoidProxy.DisposeAsync();
        }
    }
}
