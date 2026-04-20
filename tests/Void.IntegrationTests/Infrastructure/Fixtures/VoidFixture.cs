using System;
using System.IO;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class VoidFixture(PaperFixture fixture) : IAsyncLifetime
{
    public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        await fixture.EnsureInitializedAsync();
        VoidProxy = await VoidProxy.CreateAsync(Path.Combine(Path.GetTempPath(), nameof(VoidFixture), Path.GetRandomFileName()), targetServers: [$"localhost:{fixture.PaperServer1.Port}", $"localhost:{fixture.PaperServer2.Port}"], cancellationToken: Timeouts.SetupTimeoutToken); ;
    }

    public async ValueTask DisposeAsync()
    {
        await VoidProxy.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
