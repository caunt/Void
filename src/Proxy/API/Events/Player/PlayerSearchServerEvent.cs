using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Player;

public class PlayerSearchServerEvent : IEventWithResult<IServer>
{
    public required IPlayer Player { get; init; }
    public IServer? Result { get; set; }
}