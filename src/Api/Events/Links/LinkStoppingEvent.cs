using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

/// <summary>
/// Event that occurs before a link is stopped.
/// </summary>
/// <param name="Link">The link that is about to stop.</param>
/// <param name="Player">The player associated with the link.</param>
/// <param name="Reason">The reason the link is being stopped.</param>
public record LinkStoppingEvent(ILink Link, IPlayer Player, LinkStopReason Reason) : IScopedEvent;
