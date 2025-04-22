using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectedEvent(IPlayer Player) : IScopedEvent;
