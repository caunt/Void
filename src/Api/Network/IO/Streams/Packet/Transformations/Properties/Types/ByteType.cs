using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record ByteType : IPropertyType<ByteValue>
{
    public ByteValue Read(ref MinecraftBuffer buffer)
    {
        return ByteValue.FromPrimitive(buffer.ReadUnsignedByte());
    }

    public void Write(ref MinecraftBuffer buffer, ByteValue value)
    {
        buffer.WriteUnsignedByte(value.AsPrimitive);
    }
}
