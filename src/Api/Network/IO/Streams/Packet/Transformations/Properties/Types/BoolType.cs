using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record BoolType : IPropertyType<BoolValue>
{
    public BoolValue Read(ref MinecraftBuffer buffer)
    {
        return BoolValue.FromPrimitive(buffer.ReadBoolean());
    }

    public void Write(ref MinecraftBuffer buffer, BoolValue value)
    {
        buffer.WriteBoolean(value.AsPrimitive);
    }
}
