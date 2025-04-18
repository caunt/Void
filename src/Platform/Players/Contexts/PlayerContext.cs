using Void.Common.Network.Channels;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Players.Contexts;

public record PlayerContext(IServiceProvider Services) : IPlayerContext
{
    public INetworkChannel? Channel { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Channel is not null)
            await Channel.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
