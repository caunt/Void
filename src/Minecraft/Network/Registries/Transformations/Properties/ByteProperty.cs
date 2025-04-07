using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record ByteProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ByteProperty>
{
    public byte AsPrimitive => new MinecraftBuffer(Value.Span).ReadUnsignedByte();

    public static ByteProperty FromPrimitive(byte value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteUnsignedByte(value);

        return new ByteProperty(stream.ToArray());
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
