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
