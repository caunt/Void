using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Network.Channels;

/// <summary>
/// Discovers a protocol-specific channel builder and creates player-facing and server-facing channels from it.
/// </summary>
public interface IChannelBuilderService
{
    /// <summary>
    /// Gets whether discovery selected the generic fallback builder because no listener recognized the protocol.
    /// </summary>
    public bool IsFallbackBuilder { get; }

    /// <summary>
    /// Reads the player's initial data and asks channel-discovery listeners to select a builder.
    /// </summary>
    /// <param name="player">The player whose protocol and builder are being discovered.</param>
    /// <param name="cancellationToken">A token used to cancel initial network I/O and event processing.</param>
    /// <returns>A task that completes when discovery has selected either a listener-provided builder or a fallback.</returns>
    /// <exception cref="EndOfStreamException">The built-in implementation cannot read initial handshake data from the player connection.</exception>
    public ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds the player-facing channel and prepends any initial bytes consumed during discovery.
    /// </summary>
    /// <param name="player">The player that owns the channel.</param>
    /// <param name="cancellationToken">A token used to cancel channel construction.</param>
    /// <returns>The constructed player-facing channel.</returns>
    /// <exception cref="InvalidOperationException">The built-in implementation has not completed builder discovery.</exception>
    public ValueTask<INetworkChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects to a destination server and builds its server-facing channel.
    /// </summary>
    /// <param name="player">The player for whom the channel is being built.</param>
    /// <param name="server">The destination server to connect to.</param>
    /// <param name="cancellationToken">A token used to cancel connection and channel construction.</param>
    /// <returns>The constructed server-facing channel.</returns>
    /// <exception cref="InvalidOperationException">The built-in implementation has not completed builder discovery.</exception>
    public ValueTask<INetworkChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
}
