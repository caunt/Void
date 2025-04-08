using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record ShortProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ShortProperty>
{
    public short AsPrimitive => new MinecraftBuffer(Value.Span).ReadShort();

    public static ShortProperty FromPrimitive(short value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteShort(value);

        return new ShortProperty(stream.ToArray());
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
