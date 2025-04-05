using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record IntType : IPropertyType<IntValue>
{
    public IntValue Read(ref MinecraftBuffer buffer)
    {
        return IntValue.FromPrimitive(buffer.ReadInt());
    }

    public void Write(ref MinecraftBuffer buffer, IntValue value)
    {
        buffer.WriteInt(value.AsPrimitive);
    }
}
