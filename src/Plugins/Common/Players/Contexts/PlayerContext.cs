using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Players.Contexts;

internal record PlayerContext(IServiceProvider Services) : IPlayerContext
{
    // Set is allowed to upgrade the player into different implementations when required.
    public required IPlayer Player { get; internal set; }
    public INetworkChannel? Channel { get; set; }

    public void Dispose()
    {
        Channel?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (Channel is not null)
            await Channel.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
