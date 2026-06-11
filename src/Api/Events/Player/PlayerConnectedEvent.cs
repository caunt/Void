using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectedEvent(IPlayer Player) : IScopedEventWithResult<bool>
{
    public IServer? ConnectedWith { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the initial connection should be treated as anonymous.
    /// </summary>
    /// <value>
    /// <see langword="true" /> when an event listener has determined that the connection does not represent an identified player session, such as a status query; otherwise, <see langword="false" />.
    /// </value>
    public bool Result { get; set; }
}
