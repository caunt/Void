using System.Net.Sockets;
using Void.Proxy.API.Network.IO.Channels;

namespace Void.Proxy.API.Network.Protocol.Services;

public delegate ValueTask<IMinecraftChannel> ChannelBuilder(Direction direction, NetworkStream networkStream, CancellationToken cancellationToken);