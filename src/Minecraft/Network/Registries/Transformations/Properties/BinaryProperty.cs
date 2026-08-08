using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents unframed binary packet data.
/// </summary>
/// <param name="Value">The binary bytes, retained without copying.</param>
public record BinaryProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<BinaryProperty>
{
    /// <summary>
    /// Gets a read-only span over the current property memory.
    /// </summary>
    public ReadOnlySpan<byte> AsSpan => Value.Span;

    /// <summary>
    /// Creates a property over the complete contents of a memory stream without changing its position.
    /// </summary>
    /// <param name="value">The source memory stream.</param>
    /// <returns>A property that aliases the stream buffer when it is publicly visible; otherwise, a property backed by a copy.</returns>
    public static BinaryProperty FromStream(MemoryStream value)
    {
        if (value.TryGetBuffer(out var segment))
            return new BinaryProperty(segment.AsMemory());

        return new BinaryProperty(value.ToArray());
    }

    /// <summary>
    /// Consumes every remaining byte in a buffer and copies it into a binary property.
    /// </summary>
    /// <param name="buffer">The source buffer, advanced to its end.</param>
    /// <returns>A property backed by a new byte array.</returns>
    public static BinaryProperty Read(ref MinecraftBuffer buffer)
    {
        return new BinaryProperty(buffer.ReadToEnd().ToArray());
    }

    /// <summary>
    /// Writes the property's binary value to the current position of the specified buffer.
    /// </summary>
    /// <param name="buffer">The writable buffer that receives the value.</param>
    /// <remarks>
    /// The bytes are written verbatim without a length prefix, and the buffer position advances by <see cref="ReadOnlyMemory{T}.Length"/>.
    /// An empty value writes no bytes.
    /// </remarks>
    /// <exception cref="System.Data.ReadOnlyException">The buffer is backed by read-only storage.</exception>
    /// <exception cref="InternalBufferOverflowException">The writable span backing the buffer has insufficient remaining capacity.</exception>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.Write(Value.Span);
    }
}
