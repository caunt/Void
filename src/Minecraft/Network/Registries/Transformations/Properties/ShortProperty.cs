using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record ShortProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ShortProperty>
{
    public ushort AsPrimitive => new MinecraftBuffer(Value.Span).ReadUnsignedShort();

    public static ShortProperty FromPrimitive(ushort value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteUnsignedShort(value);

        return new ShortProperty(stream.ToArray());
    }

    public static ShortProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadUnsignedShort());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteUnsignedShort(AsPrimitive);
    }
}
