using System.Net.Sockets;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Common.Players;

namespace Void.Proxy.Api.Network.IO.Channels;

public delegate ValueTask<INetworkChannel> ChannelBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken);
