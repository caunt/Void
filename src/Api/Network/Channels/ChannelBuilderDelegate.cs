using System.Net.Sockets;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Network.Channels;

/// <summary>
/// Creates a network channel for one side of a player's proxied connection.
/// </summary>
/// <param name="player">The player whose connection the channel belongs to.</param>
/// <param name="side">The side of the proxied connection represented by the channel.</param>
/// <param name="networkStream">The connected stream that the channel will use for network I/O.</param>
/// <param name="cancellationToken">A token that may be used to cancel asynchronous channel creation.</param>
/// <returns>A task whose result is the channel created over <paramref name="networkStream"/>.</returns>
public delegate ValueTask<INetworkChannel> ChannelBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken);
