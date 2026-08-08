using System.Net.Sockets;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

/// <summary>
/// Requests creation of a player abstraction for an accepted TCP client.
/// </summary>
/// <param name="Client">The accepted TCP client. The resulting player assumes ownership of its connection.</param>
/// <param name="GetServices">A factory that returns the scoped service provider for a created player.</param>
public record PlayerConnectingEvent(TcpClient Client, Func<IPlayer, IServiceProvider> GetServices) : IEventWithResult<IPlayer>
{
    /// <summary>
    /// Gets or sets the player instance created for the accepted client.
    /// </summary>
    /// <value>The created player, or <see langword="null" /> when no listener has supplied one.</value>
    public IPlayer? Result { get; set; }
}
