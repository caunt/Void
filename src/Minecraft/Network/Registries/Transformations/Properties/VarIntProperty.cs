using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>Represents a Minecraft variable-length signed 32-bit packet property.</summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record VarIntProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarIntProperty>
{
    /// <summary>Gets the signed 32-bit value decoded from the variable-length bytes.</summary>
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    /// <summary>Encodes a signed 32-bit integer using Minecraft variable-length encoding.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The encoded property.</returns>
    public static VarIntProperty FromPrimitive(int value)
    {
        return new VarIntProperty(value.AsVarInt());
    }

    /// <summary>Reads one variable-length signed 32-bit integer from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    /// <exception cref="InvalidOperationException">The encoding uses more than five bytes.</exception>
    public static VarIntProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadVarInt());
    }

    /// <summary>Writes the decoded value using Minecraft variable-length encoding.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(AsPrimitive);
    }
}
