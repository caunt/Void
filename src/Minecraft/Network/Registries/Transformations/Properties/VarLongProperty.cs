using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record VarLongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarLongProperty>
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarLong();

    public static VarLongProperty FromPrimitive(long value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteVarLong(value);

        return new VarLongProperty(stream.ToArray());
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
