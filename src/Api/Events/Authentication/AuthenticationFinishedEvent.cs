using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

/// <summary>
/// Signals that an authentication attempt for a linked player has completed.
/// </summary>
/// <param name="Link">The active link whose authentication attempt completed.</param>
/// <param name="Player">The player that was authenticated.</param>
/// <param name="Side">The side that handled authentication.</param>
/// <param name="Result">The final authentication outcome.</param>
public record AuthenticationFinishedEvent(ILink Link, IPlayer Player, AuthenticationSide Side, AuthenticationResult Result) : IScopedEvent;
