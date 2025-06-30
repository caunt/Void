using System.Net.Sockets;
using Void.Proxy.Api.Events.Player;

namespace Void.Proxy.Api.Players;

/// <summary>
/// Provides operations for managing <see cref="IPlayer" /> instances.
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Gets a collection containing all active players.
    /// </summary>
    public IEnumerable<IPlayer> All { get; }

    /// <summary>
    /// Executes the specified <paramref name="action" /> for each player.
    /// </summary>
    /// <param name="action">The action to perform for each player.</param>
    public void ForEach(Action<IPlayer> action);

    /// <summary>
    /// Executes the specified asynchronous <paramref name="action" /> for each player.
    /// </summary>
    /// <param name="action">The asynchronous action to execute for each player.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public ValueTask ForEachAsync(Func<IPlayer, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Accepts a new connection and creates a player instance from the provided <see cref="TcpClient" />.
    /// </summary>
    /// <param name="client">The client representing the player connection.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upgrades the specified <paramref name="player" /> to <paramref name="upgradedPlayer" />.
    /// </summary>
    /// <param name="player">The current player instance.</param>
    /// <param name="upgradedPlayer">The upgraded player instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public ValueTask UpgradeAsync(IPlayer player, IPlayer upgradedPlayer, CancellationToken cancellationToken);

    /// <summary>
    /// Kicks a player with an optional message.
    /// </summary>
    /// <param name="player">The player to kick.</param>
    /// <param name="text">The optional kick message.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public ValueTask KickPlayerAsync(IPlayer player, string? text = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kicks a player using a <see cref="PlayerKickEvent" /> instance.
    /// </summary>
    /// <param name="player">The player to kick.</param>
    /// <param name="playerKickEvent">The event describing the kick.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public ValueTask KickPlayerAsync(IPlayer player, PlayerKickEvent playerKickEvent, CancellationToken cancellationToken = default);
}
