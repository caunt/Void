using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>Represents a one-byte Boolean packet property.</summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record BoolProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<BoolProperty>
{
    /// <summary>Gets the encoded byte converted to a Boolean.</summary>
    public bool AsPrimitive => new MinecraftBuffer(Value.Span).ReadBoolean();

    /// <summary>Encodes a Boolean as a packet property.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>A property containing <c>0</c> or <c>1</c>.</returns>
    public static BoolProperty FromPrimitive(bool value)
    {
        Span<byte> data = stackalloc byte[1];
        var buffer = new MinecraftBuffer(data);
        buffer.WriteBoolean(value);

        return new BoolProperty(data.ToArray());
    }

    /// <summary>Reads one Boolean byte from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static BoolProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadBoolean());
    }

    /// <summary>Writes the decoded Boolean as one byte.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteBoolean(AsPrimitive);
    }
}
