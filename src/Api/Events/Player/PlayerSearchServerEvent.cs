using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

public record PlayerSearchServerEvent(IPlayer Player) : IScopedEventWithResult<IServer>
{
    public IServer? Result { get; set; }
}
