using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

/// <summary>
/// Signals that a player has disconnected from the proxy.
/// </summary>
/// <param name="Player">The player that disconnected.</param>
public record PlayerDisconnectedEvent(IPlayer Player) : IScopedEvent;
