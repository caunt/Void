using System;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientFixture(PortableMinecraftClientImageFixture clientImageFixture) : IAsyncLifetime
{
    public PortableMinecraftClient Api { get => field ?? throw new InvalidOperationException($"{nameof(Api)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        Api = await PortableMinecraftClient.CreateAsync(clientImageFixture, Timeouts.SetupTimeoutToken);
    }

    public async ValueTask DisposeAsync()
    {
        await Api.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
