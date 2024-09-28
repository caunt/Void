using Microsoft.IO;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Common.Network.IO.Messages.Binary;

public class BinaryPacket(int id, RecyclableMemoryStream stream) : IMinecraftPacket
{
    public int Id => id;
    public RecyclableMemoryStream Stream => stream;

    public void Dispose()
    {
        stream.Dispose();
    }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        throw new InvalidOperationException();
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        throw new InvalidOperationException();
    }
}