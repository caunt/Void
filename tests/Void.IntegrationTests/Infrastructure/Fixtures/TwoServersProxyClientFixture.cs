using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class TwoServersProxyClientFixture(PortableMinecraftClientImageFixture clientImageFixture) : IntegrationFixtureBase, IAsyncLifetime
{
    public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }
    public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
    public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }
    public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource(SetupTimeout);

        var portableMinecraftClientTask = PortableMinecraftClient.CreateAsync(clientImageFixture, cancellationTokenSource.Token);
        var paperServer1Task = PaperServer.CreateAsync(cancellationTokenSource.Token);
        var paperServer2Task = PaperServer.CreateAsync(cancellationTokenSource.Token);

        PaperServer1 = await paperServer1Task;
        PaperServer2 = await paperServer2Task;

        var voidProxyTask = VoidProxy.CreateAsync(Path.Combine(Path.GetTempPath(), nameof(TwoServersProxyClientFixture), Path.GetRandomFileName()), targetServers: [$"localhost:{PaperServer1.Port}", $"localhost:{PaperServer2.Port}"], cancellationToken: cancellationTokenSource.Token);

        PortableMinecraftClient = await portableMinecraftClientTask;
        VoidProxy = await voidProxyTask;
    }

    public async ValueTask DisposeAsync()
    {
        await PortableMinecraftClient.DisposeAsync();
        await PaperServer1.DisposeAsync();
        await PaperServer2.DisposeAsync();
        await VoidProxy.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
