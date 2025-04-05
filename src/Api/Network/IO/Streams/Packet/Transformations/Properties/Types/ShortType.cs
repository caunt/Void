using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record ShortType : IPropertyType<ShortValue>
{
    public ShortValue Read(ref MinecraftBuffer buffer)
    {
        return ShortValue.FromPrimitive(buffer.ReadUnsignedShort());
    }

    public void Write(ref MinecraftBuffer buffer, ShortValue value)
    {
        buffer.WriteUnsignedShort(value.AsPrimitive);
    }
}
