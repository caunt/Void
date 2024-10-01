using Void.Proxy.API.Events;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Links;

public interface ILink : IEventListener, IAsyncDisposable
{
    public IPlayer Player { get; }
    public IServer Server { get; }
    public IMinecraftChannel PlayerChannel { get; }
    public IMinecraftChannel ServerChannel { get; }
    public bool IsAlive { get; }

    public ValueTask StartAsync(CancellationToken cancellationToken);
}