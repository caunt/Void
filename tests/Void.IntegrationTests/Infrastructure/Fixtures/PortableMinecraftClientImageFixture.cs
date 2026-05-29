using System;
using System.Threading.Tasks;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientImageFixture : IAsyncLifetime
{
    public const string ImageName = "ghcr.io/caunt/portable-minecraft-client:latest";

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
