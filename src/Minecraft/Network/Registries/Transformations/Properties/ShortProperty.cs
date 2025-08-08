using System;
using System.Buffers.Binary;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record ShortProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ShortProperty>
{
    public short AsPrimitive => new MinecraftBuffer(Value.Span).ReadShort();

    public static ShortProperty FromPrimitive(short value)
    {
        Span<byte> bytes = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(bytes, value);

        return new ShortProperty(bytes.ToArray());
    }

    public static ShortProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadShort());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteShort(AsPrimitive);
    }
}
