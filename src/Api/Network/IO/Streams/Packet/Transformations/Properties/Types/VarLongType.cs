using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record VarLongType : IPropertyType<VarLongValue>
{
    public VarLongValue Read(ref MinecraftBuffer buffer)
    {
        return VarLongValue.FromPrimitive(buffer.ReadVarInt());
    }

    public void Write(ref MinecraftBuffer buffer, VarLongValue value)
    {
        buffer.WriteVarLong(value.AsPrimitive);
    }
}
