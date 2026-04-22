using System;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PaperFixture : IAsyncLifetime
{
    public PaperServer Server1 { get => field ?? throw new InvalidOperationException($"{nameof(Server1)} is not initialized."); set; }
    public PaperServer Server2 { get => field ?? throw new InvalidOperationException($"{nameof(Server2)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        var paperServer1 = PaperServer.CreateAsync(Timeouts.SetupTimeoutToken);
        var paperServer2 = PaperServer.CreateAsync(Timeouts.SetupTimeoutToken);

        Server1 = await paperServer1;
        Server2 = await paperServer2;
    }

    public async ValueTask DisposeAsync()
    {
        await Server1.DisposeAsync();
        await Server2.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
