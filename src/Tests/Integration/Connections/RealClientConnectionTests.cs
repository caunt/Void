using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Integration.Base;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections;

public class RealClientConnectionTests(RealClientConnectionTests.Fixture fixture) : IntegrationUnitBase, IClassFixture<RealClientConnectionTests.Fixture>
{
    private const int ProxyPort = 37000;
    private const int ServerPort = 37001;

    [Fact]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy()
    {
        await LoggedExecutorAsync(() =>
        {
            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(PortableMinecraftClient.Username));
            return Task.CompletedTask;
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class Fixture : IntegrationFixtureBase, IAsyncLifetime
    {
        private PortableMinecraftClient? _portableMinecraftClient;
        private PaperServer? _paperServer;
        private VoidProxy? _voidProxy;

        public Fixture() : base(nameof(RealClientConnectionTests))
        {
        }

        public PortableMinecraftClient PortableMinecraftClient => _portableMinecraftClient ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized.");
        public PaperServer PaperServer => _paperServer ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized.");
        public VoidProxy VoidProxy => _voidProxy ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized.");

        public async Task InitializeAsync()
        {
            using var setupCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(30));
            var setupCancellationToken = setupCancellationTokenSource.Token;

            var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(_workingDirectory, setupCancellationToken);
            var paperServerTask = PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: setupCancellationToken);
            
            _portableMinecraftClient = await portableMinecraftClientTask;
            _paperServer = await paperServerTask;
            _voidProxy = await VoidProxy.CreateAsync(_workingDirectory, targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: setupCancellationToken);

            await _portableMinecraftClient.StartConnectingAsync($"localhost:{ProxyPort}", setupCancellationToken);
            await _paperServer.ExpectTextAsync(PortableMinecraftClient.Username, lookupHistory: true, setupCancellationToken);
        }

        public async Task DisposeAsync()
        {
            if (_portableMinecraftClient is not null)
                await _portableMinecraftClient.DisposeAsync();

            if (_paperServer is not null)
                await _paperServer.DisposeAsync();

            if (_voidProxy is not null)
                await _voidProxy.DisposeAsync();
        }
    }
}
