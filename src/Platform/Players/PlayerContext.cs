using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;

namespace Void.Proxy.Players;

public class PlayerContext(IServiceProvider services) : IPlayerContext
{
    public IServiceProvider Services => services;

    public IMinecraftChannel? Channel { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Channel is not null)
            await Channel.DisposeAsync();
    }
}
