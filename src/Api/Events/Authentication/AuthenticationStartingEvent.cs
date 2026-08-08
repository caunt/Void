using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

/// <summary>
/// Requests a decision about which side should authenticate a linked player.
/// </summary>
/// <param name="Link">The active link awaiting authentication.</param>
/// <param name="Player">The player whose authentication side is being selected.</param>
public record AuthenticationStartingEvent(ILink Link, IPlayer Player) : IScopedEventWithResult<AuthenticationSide>
{
    /// <summary>
    /// Gets or sets the side selected to handle authentication.
    /// </summary>
    /// <value>
    /// The selected authentication side. The default value is <see cref="AuthenticationSide.Server" /> when no listener assigns a result.
    /// </value>
    public AuthenticationSide Result { get; set; }
}
