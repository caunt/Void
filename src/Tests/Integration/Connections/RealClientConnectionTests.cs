using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Integration.Base;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;
using Xunit.Sdk;

namespace Void.Tests.Integration.Connections;

public class RealClientConnectionTests(RealClientConnectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<RealClientConnectionTests.Fixture>
{
    private const int ProxyPort = 37000;
    private const int ServerPort = 37001;
    private const string ExpectedText = "hello proxied void!";

    [Fact]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TestTimeout);
        
        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.SendTextMessageAsync(new DnsEndPoint("localhost", ProxyPort), expectedText, cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class Fixture() : IntegrationFixtureBase(nameof(RealClientConnectionTests)), IAsyncLifetime
    {
        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            using var setupCancellationTokenSource = new CancellationTokenSource(SetupTimeout);
            var setupCancellationToken = setupCancellationTokenSource.Token;

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(_workingDirectory, setupCancellationToken);
            var paperServerTask = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: setupCancellationToken);
            var voidProxyTask = VoidProxy.CreateAsync(_workingDirectory, targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: setupCancellationToken);
            
            PortableMinecraftClient = await portableMinecraftClientTask;
            PaperServer = await paperServerTask;
            VoidProxy = await voidProxyTask;
        }

        public async Task DisposeAsync()
        {
            await PortableMinecraftClient.DisposeAsync();
            await PaperServer.DisposeAsync();
            await VoidProxy.DisposeAsync();
        }
    }
}
