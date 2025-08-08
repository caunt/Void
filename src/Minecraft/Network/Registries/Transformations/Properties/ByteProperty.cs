using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record ByteProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ByteProperty>
{
    public byte AsPrimitive => new MinecraftBuffer(Value.Span).ReadUnsignedByte();

    public static ByteProperty FromPrimitive(byte value)
    {
        Span<byte> span = stackalloc byte[1];
        var buffer = new MinecraftBuffer(span);
        buffer.WriteUnsignedByte(value);

        return new ByteProperty(span.ToArray());
    }

    public static ByteProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadUnsignedByte());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteUnsignedByte(AsPrimitive);
    }
}
