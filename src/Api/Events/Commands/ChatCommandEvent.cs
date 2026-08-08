using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Commands;

/// <summary>
/// Represents a chat command received from a linked player before it is forwarded to the server.
/// </summary>
/// <param name="Link">The active player-to-server link.</param>
/// <param name="Player">The player that submitted the command.</param>
/// <param name="Command">The command text without the leading command delimiter.</param>
/// <param name="IsSigned"><see langword="true" /> when the received command carries a protocol signature; otherwise, <see langword="false" />.</param>
public record ChatCommandEvent(ILink Link, IPlayer Player, string Command, bool IsSigned) : IScopedEventWithResult<bool>
{
    /// <summary>
    /// Gets or sets whether forwarding the command to the destination server is suppressed.
    /// </summary>
    /// <value><see langword="true" /> to consume the command at the proxy; otherwise, <see langword="false" />.</value>
    public bool Result { get; set; }
}
