using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerDisconnectedEvent(IPlayer Player) : IEvent;
