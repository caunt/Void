using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Player;

public class PlayerDisconnectedEvent : IEvent
{
    public required IPlayer Player { get; init; }
}