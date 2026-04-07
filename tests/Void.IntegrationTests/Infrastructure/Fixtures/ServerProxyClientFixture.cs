using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class ServerProxyClientFixture(PortableMinecraftClientImageFixture clientImageFixture) : IntegrationFixtureBase, IAsyncLifetime
{
    public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
    public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
    public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

        var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(clientImageFixture, cancellationTokenSource.Token);
        var paperServerTask = PaperServer.CreateAsync(cancellationTokenSource.Token);

        PaperServer = await paperServerTask;
        var voidProxyTask = VoidProxy.CreateAsync(Path.Combine(Path.GetTempPath(), nameof(ServerProxyClientFixture), Path.GetRandomFileName()), targetServer: $"localhost:{PaperServer.Port}", cancellationToken: cancellationTokenSource.Token);

        PortableMinecraftClient = await portableMinecraftClientTask;
        VoidProxy = await voidProxyTask;
    }

    public async ValueTask DisposeAsync()
    {
        await PortableMinecraftClient.DisposeAsync();
        await PaperServer.DisposeAsync();
        await VoidProxy.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
