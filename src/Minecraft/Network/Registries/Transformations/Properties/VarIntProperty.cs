using System;
using System.Linq;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record VarIntProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarIntProperty>
{
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarIntProperty FromPrimitive(int value)
    {
        return new VarIntProperty(MinecraftBuffer.EnumerateVarInt(value).ToArray());
    }

    public static VarIntProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadVarInt());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(AsPrimitive);
    }
}
