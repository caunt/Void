using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents a 64-bit signed integer property encoded as the Minecraft long binary format.
/// </summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record LongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<LongProperty>
{
    /// <summary>Gets the signed 64-bit value decoded from the property bytes.</summary>
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadLong();

    /// <summary>Encodes a signed 64-bit integer as eight big-endian bytes.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The encoded property.</returns>
    public static LongProperty FromPrimitive(long value)
    {
        var array = GC.AllocateUninitializedArray<byte>(8);
        var span = array.AsSpan();
        var buffer = new MinecraftBuffer(span);
        buffer.WriteLong(value);

        return new LongProperty(array);
    }
    /// <summary>Reads one big-endian signed 64-bit integer from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static LongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadLong());
    }

    /// <summary>Writes the decoded signed 64-bit integer in big-endian order.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteLong(AsPrimitive);
    }
}
