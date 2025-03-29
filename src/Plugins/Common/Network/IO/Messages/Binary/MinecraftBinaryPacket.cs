using Microsoft.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Binary;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.IO.Messages.Binary;

public class MinecraftBinaryPacket(int id, RecyclableMemoryStream stream) : IMinecraftBinaryMessage, IMinecraftServerboundPacket, IMinecraftClientboundPacket, IMinecraftPacket
{
    public int Id => id;
    public MemoryStream Stream => stream;

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