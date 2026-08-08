using System;
using System.Buffers.Binary;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>Represents a big-endian signed 16-bit packet property.</summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record ShortProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ShortProperty>
{
    /// <summary>Gets the signed 16-bit value decoded from the property bytes.</summary>
    public short AsPrimitive => new MinecraftBuffer(Value.Span).ReadShort();

    /// <summary>Encodes a signed 16-bit integer as two big-endian bytes.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The encoded property.</returns>
    public static ShortProperty FromPrimitive(short value)
    {
        Span<byte> bytes = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(bytes, value);

        return new ShortProperty(bytes.ToArray());
    }

    /// <summary>Reads one big-endian signed 16-bit integer from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static ShortProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadShort());
    }

    /// <summary>Writes the decoded signed 16-bit integer in big-endian order.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteShort(AsPrimitive);
    }
}
