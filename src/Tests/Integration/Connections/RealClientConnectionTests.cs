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

    [RealClientFact]
    public async Task PortableMinecraftClientConnectsToPaperServerThroughProxy()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        await LoggedExecutorAsync(async () =>
        {
            await fixture.PortableMinecraftClient.StartConnectingAsync($"localhost:{ProxyPort}", cancellationTokenSource.Token);
            await fixture.PaperServer.ExpectTextAsync(PortableMinecraftClient.Username, lookupHistory: true, cancellationTokenSource.Token);

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(PortableMinecraftClient.Username));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }

    public class Fixture : IntegrationFixtureBase, IAsyncLifetime
    {
        public Fixture() : base(nameof(RealClientConnectionTests))
        {
        }

        public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
        public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
        public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

        public async Task InitializeAsync()
        {
            if (!IntegrationTestEnvironment.RealClientTestsEnabled)
                return;

            using var buildCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(30));
            PortableMinecraftClient = await PortableMinecraftClient.CreateAsync(_workingDirectory, buildCancellationTokenSource.Token);

            using var cancellationTokenSource = new CancellationTokenSource(Timeout);
            PaperServer = await PaperServer.CreateAsync(_workingDirectory, _httpClient, port: ServerPort, cancellationToken: cancellationTokenSource.Token);
            VoidProxy = await VoidProxy.CreateAsync(_workingDirectory, targetServer: $"localhost:{ServerPort}", proxyPort: ProxyPort, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (!IntegrationTestEnvironment.RealClientTestsEnabled)
                return;

            if (PortableMinecraftClient is not null)
                await PortableMinecraftClient.DisposeAsync();

            if (PaperServer is not null)
                await PaperServer.DisposeAsync();

            if (VoidProxy is not null)
                await VoidProxy.DisposeAsync();
        }
    }
}
