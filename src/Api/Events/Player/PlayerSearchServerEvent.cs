using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

/// <summary>
/// Requests a destination server for a player connection.
/// </summary>
/// <param name="Player">The player awaiting a destination server.</param>
/// <param name="ConnectedWith">The server preferred by earlier connection processing, or <see langword="null" /> when no preference exists.</param>
public record PlayerSearchServerEvent(IPlayer Player, IServer? ConnectedWith) : IScopedEventWithResult<IServer>
{
    /// <summary>
    /// Gets or sets the destination server selected by a listener.
    /// </summary>
    /// <value>The selected server, or <see langword="null" /> to continue with the platform's normal candidate selection.</value>
    public IServer? Result { get; set; }
}
