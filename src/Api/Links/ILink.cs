using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Links;

/// <summary>
/// Represents an active bidirectional forwarding session between a player and a backend Minecraft server.
/// </summary>
/// <remarks>
/// A link owns two <see cref="INetworkChannel"/> instances — one toward the player and one toward the server —
/// and drives two concurrent forwarding loops that relay <see cref="Void.Proxy.Api.Network.Messages.INetworkMessage"/>
/// objects between them. Each inbound message is surfaced through the event system before being written to the
/// opposite channel, allowing plugins to inspect or cancel individual packets.
/// The link stops naturally when either side closes its connection, and raises
/// <see cref="Void.Proxy.Api.Events.Links.LinkStoppingEvent"/> followed by
/// <see cref="Void.Proxy.Api.Events.Links.LinkStoppedEvent"/> with the appropriate <see cref="LinkStopReason"/>.
/// </remarks>
public interface ILink : IEventListener, IAsyncDisposable
{
    /// <summary>
    /// Gets the player whose client connection is being proxied through this link.
    /// </summary>
    public IPlayer Player { get; }

    /// <summary>
    /// Gets the backend server that the player is currently connected to through this link.
    /// </summary>
    public IServer Server { get; }

    /// <summary>
    /// Gets the network channel that faces the player's client.
    /// </summary>
    public INetworkChannel PlayerChannel { get; }

    /// <summary>
    /// Gets the network channel that faces the backend server.
    /// </summary>
    public INetworkChannel ServerChannel { get; }

    /// <summary>
    /// Gets a value indicating whether both forwarding tasks are still running.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if neither the serverbound nor the clientbound forwarding task has completed;
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool IsAlive { get; }

    /// <summary>
    /// Gets an enumerable containing both <see cref="PlayerChannel"/> and <see cref="ServerChannel"/>.
    /// </summary>
    public IEnumerable<INetworkChannel> Channels => [PlayerChannel, ServerChannel];

    /// <summary>
    /// Starts the bidirectional forwarding loops that relay network messages between the player and the server.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the start operation.</param>
    /// <exception cref="InvalidOperationException">Thrown when the link has already been started.</exception>
    public ValueTask StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Requests an orderly shutdown of both forwarding loops and waits for them to drain.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the stop operation.</param>
    /// <exception cref="InvalidOperationException">Thrown when the link has not been started yet.</exception>
    public ValueTask StopAsync(CancellationToken cancellationToken);
}
