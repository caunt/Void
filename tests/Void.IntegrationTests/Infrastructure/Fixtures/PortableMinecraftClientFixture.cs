using System;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientFixture(PortableMinecraftClientImageFixture clientImageFixture) : IAsyncLifetime
{
    public PortableMinecraftClient PortableMinecraftClient { get => field ?? throw new InvalidOperationException($"{nameof(PortableMinecraftClient)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        PortableMinecraftClient = await PortableMinecraftClient.CreateAsync(clientImageFixture, Timeouts.SetupTimeoutToken);
    }

    public async ValueTask DisposeAsync()
    {
        await PortableMinecraftClient.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
