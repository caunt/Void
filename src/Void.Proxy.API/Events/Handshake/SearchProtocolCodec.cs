using System.Net.Sockets;
using Void.Proxy.API.Network.IO.Channels;

namespace Void.Proxy.API.Events.Handshake;

public class SearchProtocolCodec : IEventWithResult<SearchProtocolCodec.Data>
{
    public required Memory<byte> Buffer { get; init; }
    public Data? Result { get; set; }

    public class Data
    {
        public required Func<NetworkStream, Task<IMinecraftChannel>> ChannelBuilder { get; set; }
    }
}