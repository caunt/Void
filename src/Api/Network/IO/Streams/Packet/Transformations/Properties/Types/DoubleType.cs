using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record DoubleType : IPropertyType<DoubleValue>
{
    public DoubleValue Read(ref MinecraftBuffer buffer)
    {
        return DoubleValue.FromPrimitive(buffer.ReadDouble());
    }

    public void Write(ref MinecraftBuffer buffer, DoubleValue value)
    {
        buffer.WriteDouble(value.AsPrimitive);
    }
}
