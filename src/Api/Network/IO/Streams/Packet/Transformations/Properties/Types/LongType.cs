using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record LongType : IPropertyType<LongValue>
{
    public LongValue Read(ref MinecraftBuffer buffer)
    {
        return LongValue.FromPrimitive(buffer.ReadLong());
    }

    public void Write(ref MinecraftBuffer buffer, LongValue value)
    {
        buffer.WriteLong(value.AsPrimitive);
    }
}
