using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

/// <summary>
/// Signals that a player-to-server link is about to begin relaying messages.
/// </summary>
/// <param name="Link">The link entering its running state.</param>
/// <param name="Player">The player associated with the link.</param>
public record LinkStartingEvent(ILink Link, IPlayer Player) : IScopedEvent;
