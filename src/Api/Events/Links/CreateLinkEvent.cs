using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Links;

/// <summary>
/// Requests creation of the link that will relay traffic between a player and a destination server.
/// </summary>
/// <param name="Player">The player to associate with the link.</param>
/// <param name="Server">The destination server to associate with the link.</param>
/// <param name="PlayerChannel">The player-facing network channel.</param>
/// <param name="ServerChannel">The destination-server network channel.</param>
public record CreateLinkEvent(IPlayer Player, IServer Server, INetworkChannel PlayerChannel, INetworkChannel ServerChannel) : IScopedEventWithResult<ILink>
{
    /// <summary>
    /// Gets or sets a custom link created by a listener.
    /// </summary>
    /// <value>The link to use, or <see langword="null" /> to let the platform create its default link.</value>
    public ILink? Result { get; set; }
}
