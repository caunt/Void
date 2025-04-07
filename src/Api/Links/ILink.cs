using Void.Common.Events;
using Void.Common.Network.Channels;
using Void.Common.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Links;

public interface ILink : IEventListener, IAsyncDisposable
{
    public IPlayer Player { get; }
    public IServer Server { get; }
    public INetworkChannel PlayerChannel { get; }
    public INetworkChannel ServerChannel { get; }
    public bool IsAlive { get; }

    public ValueTask StartAsync(CancellationToken cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken);
}
