using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents the encoded payload of a fixed-width integer packet property.
/// </summary>
/// <param name="Value">The bytes containing the big-endian Minecraft binary representation of the integer.</param>
/// <remarks>
/// The supplied memory is stored without validation. It must contain at least four bytes before the value is decoded or written.
/// </remarks>
public record IntProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<IntProperty>
{
    /// <summary>Gets the signed 32-bit value decoded from the property bytes.</summary>
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadInt();

    /// <summary>Encodes a signed 32-bit integer as four big-endian bytes.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The encoded property.</returns>
    public static IntProperty FromPrimitive(int value)
    {
        Span<byte> span = stackalloc byte[4];
        var buffer = new MinecraftBuffer(span);
        buffer.WriteInt(value);

        return new IntProperty(span.ToArray());
    }

    /// <summary>Reads one big-endian signed 32-bit integer from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static IntProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadInt());
    }

    /// <summary>Writes the decoded signed 32-bit integer in big-endian order.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteInt(AsPrimitive);
    }
}
