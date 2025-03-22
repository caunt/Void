using Microsoft.IO;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages.Binary;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.IO.Messages.Binary;

public class MinecraftBinaryPacket(int id, RecyclableMemoryStream stream) : IBinaryMessage, IMinecraftServerboundPacket, IMinecraftClientboundPacket, IMinecraftPacket
{
    public int Id => id;
    public RecyclableMemoryStream Stream => stream;

    public void Dispose()
    {
        stream.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        throw new InvalidOperationException();
    }

    public static void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        throw new InvalidOperationException();
    }
}