using Void.Proxy.API.Network.IO;

namespace Void.Proxy.API.Events.Handshake;

public class SearchClientProtocolCodec : IEventWithResult<SearchClientProtocolCodec.Data>
{
    public required Memory<byte> Buffer { get; init; }
    public Data? Result { get; set; }

    public class Data
    {
        public required IMinecraftChannel Channel { get; set; }
        public required Memory<byte> NextBuffer { get; set; }
    }
}
