using System.Net.Sockets;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Players;

/// <summary>
/// Represents a connected player and exposes information about their network
/// connection and execution context.
/// </summary>
public interface IPlayer : IEquatable<IPlayer>, ICommandSource, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets the underlying <see cref="TcpClient"/> used for network.
    /// communication.
    /// </summary>
    public TcpClient Client { get; }

    /// <summary>
    /// Gets the name of the player. Fallbacks to IP address if unavailable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the textual representation of the client's remote endpoint.
    /// </summary>
    public string RemoteEndPoint { get; }

    /// <summary>
    /// Gets the date and time when the player connected to the proxy.
    /// </summary>
    public DateTimeOffset ConnectedAt { get; }

    /// <summary>
    /// Gets the context containing services and state associated with the
    /// player.
    /// </summary>
    public IPlayerContext Context { get; }

    /// <summary>
    /// Computes a stable hash code for the current instance. This is useful when the instance is upgraded or replaced to another implementation.
    /// </summary>
    /// <remarks>The hash code is based on the <see cref="Client"/> property.</remarks>
    /// <returns>An integer representing the hash code of the current instance, derived from the <see cref="Client"/> property.</returns>
    public int GetStableHashCode()
    {
        return Client.GetHashCode();
    }
}
