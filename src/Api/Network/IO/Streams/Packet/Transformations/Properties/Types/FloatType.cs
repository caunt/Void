using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record FloatType : IPropertyType<FloatValue>
{
    public FloatValue Read(ref MinecraftBuffer buffer)
    {
        return FloatValue.FromPrimitive(buffer.ReadFloat());
    }

    public void Write(ref MinecraftBuffer buffer, FloatValue value)
    {
        buffer.WriteFloat(value.AsPrimitive);
    }
}
