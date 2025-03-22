using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Player;

public record PlayerKickEvent(IPlayer Player, string? Reason = null) : IEvent;