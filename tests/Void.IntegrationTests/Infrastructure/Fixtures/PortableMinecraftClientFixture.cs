using System;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientFixture : IAsyncLifetime
{
    private PortableMinecraftClient? _api;

    public PortableMinecraftClient Api => _api ?? throw new InvalidOperationException($"{nameof(Api)} is not initialized.");

    public async ValueTask InitializeAsync()
    {
        _api = await PortableMinecraftClient.CreateAsync(Timeouts.SetupTimeoutToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_api is not null)
            await _api.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
