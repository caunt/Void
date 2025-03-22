using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Player;

public record PlayerSearchServerEvent(IPlayer Player) : IEventWithResult<IServer>
{
    public IServer? Result { get; set; }
}