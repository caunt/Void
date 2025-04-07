using System;
using System.Linq;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record VarLongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarLongProperty>
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarLongProperty FromPrimitive(int value)
    {
        return new VarLongProperty(MinecraftBuffer.EnumerateVarInt(value).ToArray());
    }

    public static VarLongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadVarInt());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarLong(AsPrimitive);
    }
}
