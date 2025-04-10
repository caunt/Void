using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record VarLongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarLongProperty>
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarLongProperty FromPrimitive(int value)
    {
        return new VarLongProperty(value.AsVarInt());
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
