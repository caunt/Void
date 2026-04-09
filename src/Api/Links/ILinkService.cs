using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Links;

/// <summary>
/// Manages the lifecycle of <see cref="ILink"/> instances that connect players to backend servers,
/// and provides methods to establish or look up player-to-server connections.
/// </summary>
public interface ILinkService
{
    /// <summary>
    /// Gets all currently active links — links that have completed authentication and are
    /// actively forwarding traffic between a player and a backend server.
    /// </summary>
    public IReadOnlyList<ILink> All { get; }

    /// <summary>
    /// Connects the specified player to the first available backend server by firing a
    /// <see cref="Events.Player.PlayerSearchServerEvent"/> to let plugins nominate a preferred server.
    /// If no plugin selects a server, every configured server is tried in order until one accepts the player.
    /// </summary>
    /// <param name="player">The player to connect.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// <see cref="ConnectionResult.Connected"/> if a server was found and the player authenticated successfully;
    /// <see cref="ConnectionResult.NotConnected"/> if no available server accepted the player.
    /// </returns>
    public ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects the specified player to the first available backend server that is not in
    /// <paramref name="ignoredServers"/> by firing a <see cref="Events.Player.PlayerSearchServerEvent"/>
    /// to let plugins nominate a preferred server.
    /// If no plugin selects a server, every configured server not in the ignore list is tried in order.
    /// </summary>
    /// <param name="player">The player to connect.</param>
    /// <param name="ignoredServers">Servers that must not be considered during this connection attempt.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// <see cref="ConnectionResult.Connected"/> if a server was found and the player authenticated successfully;
    /// <see cref="ConnectionResult.NotConnected"/> if no available server accepted the player.
    /// </returns>
    public ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, IEnumerable<IServer> ignoredServers, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects the specified player to the given backend server.
    /// If the target server is unreachable, the player is redirected back to their previous server.
    /// If the previous server is also unreachable, the player is kicked with an explanatory message.
    /// </summary>
    /// <param name="player">The player to connect.</param>
    /// <param name="server">The target backend server.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// <see cref="ConnectionResult.Connected"/> if the player connected to the target or the previous server;
    /// <see cref="ConnectionResult.NotConnected"/> if neither the target nor the previous server could be reached.
    /// </returns>
    public ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to retrieve the active link for the specified player.
    /// A link is considered active once authentication has completed and traffic forwarding has started.
    /// </summary>
    /// <param name="player">The player whose active link to look up.</param>
    /// <param name="link">
    /// When this method returns <see langword="true"/>, contains the player's active link;
    /// otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if an active link was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link);

    /// <summary>
    /// Attempts to retrieve a link for the specified player that is currently in the authentication phase
    /// and has not yet been promoted to an active link.
    /// A weak link is created at the start of the connection attempt and exists until authentication
    /// either succeeds — at which point it also becomes an active link — or fails.
    /// </summary>
    /// <param name="player">The player whose pending link to look up.</param>
    /// <param name="link">
    /// When this method returns <see langword="true"/>, contains the player's pending link;
    /// otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if a pending link was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetWeakLink(IPlayer player, [NotNullWhen(true)] out ILink? link);

    /// <summary>
    /// Determines whether the specified player has an active link to a backend server.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns><see langword="true"/> if the player has an active link; otherwise, <see langword="false"/>.</returns>
    public bool HasLink(IPlayer player);
}
