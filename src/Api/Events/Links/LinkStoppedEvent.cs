using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

/// <summary>
/// Signals that a player-to-server link has stopped relaying messages.
/// </summary>
/// <param name="Link">The link that stopped.</param>
/// <param name="Player">The player associated with the link.</param>
/// <param name="Reason">The condition that caused the link to stop.</param>
public record LinkStoppedEvent(ILink Link, IPlayer Player, LinkStopReason Reason) : IScopedEvent;
