using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record VarIntType : IPropertyType<VarIntValue>
{
    public VarIntValue Read(ref MinecraftBuffer buffer)
    {
        return VarIntValue.FromPrimitive(buffer.ReadVarInt());
    }

    public void Write(ref MinecraftBuffer buffer, VarIntValue value)
    {
        System.Console.WriteLine($"Write VarInt: {value.AsPrimitive}");
        buffer.WriteVarInt(value.AsPrimitive);
    }
}
