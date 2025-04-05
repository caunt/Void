using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record UuidType : IPropertyType<UuidValue>
{
    public UuidValue Read(ref MinecraftBuffer buffer)
    {
        return UuidValue.FromPrimitive(buffer.ReadUuid());
    }

    public void Write(ref MinecraftBuffer buffer, UuidValue value)
    {
        buffer.WriteUuid(value.AsPrimitive);
    }
}
