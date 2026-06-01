using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectedEvent(IPlayer Player) : IScopedEventWithResult<bool>
{
    public IServer? ConnectedWith { get; set; }
    public bool Result { get; set; }
}
