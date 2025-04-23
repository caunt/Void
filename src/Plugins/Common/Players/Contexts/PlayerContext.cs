using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Players.Contexts;

public record PlayerContext(IPlayer Player, AsyncServiceScope Scope, IServiceProvider Services) : IPlayerContext
{
    public INetworkChannel? Channel { get; set; }

    public void Dispose()
    {
        Channel?.Dispose();
        Scope.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (Channel is not null)
            await Channel.DisposeAsync();

        await Scope.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
