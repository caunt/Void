using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>Represents a big-endian IEEE 754 double-precision packet property.</summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record DoubleProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<DoubleProperty>
{
    /// <summary>Gets the double-precision value decoded from the property bytes.</summary>
    public double AsPrimitive => new MinecraftBuffer(Value.Span).ReadDouble();

    /// <summary>Encodes a double-precision value as a packet property.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>A property containing eight big-endian bytes.</returns>
    public static DoubleProperty FromPrimitive(double value)
    {
        Span<byte> bytes = stackalloc byte[sizeof(double)];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteDouble(value);
        return new DoubleProperty(bytes.ToArray());
    }

    /// <summary>Reads one big-endian double-precision value from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static DoubleProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadDouble());
    }

    /// <summary>Writes the decoded double-precision value in big-endian order.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteDouble(AsPrimitive);
    }
}
