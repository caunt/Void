using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record ShortProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ShortProperty>
{
    public short AsPrimitive => new MinecraftBuffer(Value.Span).ReadShort();

    public static ShortProperty FromPrimitive(short value)
    {
        Span<byte> bytes = stackalloc byte[2];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteShort(value);

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
