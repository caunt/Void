using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

/// <summary>
/// Signals that a player-to-server link has begun relaying messages.
/// </summary>
/// <param name="Link">The link that started.</param>
/// <param name="Player">The player associated with the link.</param>
/// <param name="IsFirstLink"><see langword="true" /> when this is the player's initial server link; <see langword="false" /> for a redirection.</param>
/// <param name="IsAnonymous"><see langword="true" /> when the connection does not represent an authenticated player session; otherwise, <see langword="false" />.</param>
public record LinkStartedEvent(ILink Link, IPlayer Player, bool IsFirstLink, bool IsAnonymous) : IScopedEvent;
