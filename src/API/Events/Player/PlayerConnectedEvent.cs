using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Player;

public class PlayerConnectedEvent : IEvent
{
    public required IPlayer Player { get; init; }
}