using System.Net.Sockets;

namespace Void.Proxy.API.Network.IO.Channels.Services;

public delegate ValueTask<IMinecraftChannel> ChannelBuilder(Direction direction, NetworkStream networkStream, CancellationToken cancellationToken);