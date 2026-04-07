using System;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class ServerClientFixture(PortableMinecraftClientImageFixture clientImageFixture) : IntegrationFixtureBase, IAsyncLifetime
{
    public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
    public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

        var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(clientImageFixture, cancellationTokenSource.Token);
        var paperServerTask = PaperServer.CreateAsync(cancellationToken: cancellationTokenSource.Token);

        PortableMinecraftClient = await portableMinecraftClientTask;
        PaperServer = await paperServerTask;
    }

    public async ValueTask DisposeAsync()
    {
        await PortableMinecraftClient.DisposeAsync();
        await PaperServer.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
