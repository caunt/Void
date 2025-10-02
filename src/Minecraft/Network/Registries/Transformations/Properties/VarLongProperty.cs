using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record VarLongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarLongProperty>
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarLong();

    public static VarLongProperty FromPrimitive(long value)
    {
        Span<byte> bytes = stackalloc byte[10];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteVarLong(value);

        return new VarLongProperty(bytes[..(int)buffer.Position].ToArray());
    }

    public static VarLongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadVarLong());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarLong(AsPrimitive);
    }
}
