using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

/// <summary>
/// Signals that a player connection has been accepted and is ready for initial server selection.
/// </summary>
/// <param name="Player">The connected player.</param>
public record PlayerConnectedEvent(IPlayer Player) : IScopedEventWithResult<bool>
{
    /// <summary>
    /// Gets or sets the server preferred by a listener for the player's initial connection.
    /// </summary>
    /// <value>The preferred server, or <see langword="null" /> to use normal server selection.</value>
    public IServer? ConnectedWith { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the initial connection should be treated as anonymous.
    /// </summary>
    /// <value>
    /// <see langword="true" /> when an event listener has determined that the connection does not represent an identified player session, such as a status query; otherwise, <see langword="false" />.
    /// </value>
    public bool Result { get; set; }
}
