using System.Net.Sockets;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Network.IO.Channels.Services;

public delegate ValueTask<IMinecraftChannel> ChannelBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken);