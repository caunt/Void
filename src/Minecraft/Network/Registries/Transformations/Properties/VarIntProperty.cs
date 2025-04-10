using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record VarIntProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarIntProperty>
{
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarIntProperty FromPrimitive(int value)
    {
        return new VarIntProperty(value.AsVarInt());
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
