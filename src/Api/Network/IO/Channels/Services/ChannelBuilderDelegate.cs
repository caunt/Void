using System.Net.Sockets;
using Void.Common;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Network.IO.Channels.Services;

public delegate ValueTask<IMinecraftChannel> ChannelBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken);
