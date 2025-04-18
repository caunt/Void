using System.Net.Sockets;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Network.Channels;

public delegate ValueTask<INetworkChannel> ChannelBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken);
