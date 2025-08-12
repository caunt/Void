using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record BinaryProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<BinaryProperty>
{
    public ReadOnlySpan<byte> AsSpan => Value.Span;

    public static BinaryProperty FromStream(MemoryStream value)
    {
        if (value.TryGetBuffer(out var segment))
            return new BinaryProperty(segment.AsMemory());

        return new BinaryProperty(value.ToArray());
    }

    public static BinaryProperty Read(ref MinecraftBuffer buffer)
    {
        return new BinaryProperty(buffer.ReadToEnd().ToArray());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.Write(Value.Span);
    }
}
