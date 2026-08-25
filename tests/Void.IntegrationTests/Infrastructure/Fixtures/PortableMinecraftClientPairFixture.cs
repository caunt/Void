using System;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientPairFixture : IAsyncLifetime
{
    private PortableMinecraftClient? _client1;
    private PortableMinecraftClient? _client2;

    public PortableMinecraftClient Client1 => _client1 ?? throw new InvalidOperationException($"{nameof(Client1)} is not initialized.");
    public PortableMinecraftClient Client2 => _client2 ?? throw new InvalidOperationException($"{nameof(Client2)} is not initialized.");

    public async ValueTask InitializeAsync()
    {
        var firstClient = PortableMinecraftClient.CreateAsync(Timeouts.SetupTimeoutToken);
        var secondClient = PortableMinecraftClient.CreateAsync(Timeouts.SetupTimeoutToken);

        try
        {
            await Task.WhenAll(firstClient, secondClient);
            _client1 = await firstClient;
            _client2 = await secondClient;
        }
        catch
        {
            if (firstClient.IsCompletedSuccessfully)
                await firstClient.Result.DisposeAsync();

            if (secondClient.IsCompletedSuccessfully)
                await secondClient.Result.DisposeAsync();

            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_client1 is not null)
            await _client1.DisposeAsync();

        if (_client2 is not null)
            await _client2.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
