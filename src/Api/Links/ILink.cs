using Void.Common;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Links;

public interface ILink : IEventListener, IAsyncDisposable
{
    public IPlayer Player { get; }
    public IServer Server { get; }
    public IMinecraftChannel PlayerChannel { get; }
    public IMinecraftChannel ServerChannel { get; }
    public bool IsAlive { get; }

    public ValueTask StartAsync(CancellationToken cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken);
}
