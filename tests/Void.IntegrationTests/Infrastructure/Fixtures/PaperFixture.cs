using System;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PaperFixture : IAsyncLifetime
{
    public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
    public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        var paperServer1 = PaperServer.CreateAsync(Timeouts.SetupTimeoutToken);
        var paperServer2 = PaperServer.CreateAsync(Timeouts.SetupTimeoutToken);

        PaperServer1 = await paperServer1;
        PaperServer2 = await paperServer2;
    }

    public async ValueTask DisposeAsync()
    {
        await PaperServer1.DisposeAsync();
        await PaperServer2.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
