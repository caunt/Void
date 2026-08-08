using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>Represents a one-byte unsigned packet property.</summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record ByteProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ByteProperty>
{
    /// <summary>Gets the first encoded byte.</summary>
    public byte AsPrimitive => new MinecraftBuffer(Value.Span).ReadUnsignedByte();

    /// <summary>Encodes an unsigned byte as a packet property.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>A property containing the byte.</returns>
    public static ByteProperty FromPrimitive(byte value)
    {
        Span<byte> span = stackalloc byte[1];
        var buffer = new MinecraftBuffer(span);
        buffer.WriteUnsignedByte(value);

        return new ByteProperty(span.ToArray());
    }

    /// <summary>Reads one unsigned byte from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static ByteProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadUnsignedByte());
    }

    /// <summary>Writes the decoded unsigned byte.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteUnsignedByte(AsPrimitive);
    }
}
