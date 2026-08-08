using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>Represents a Minecraft variable-length signed 64-bit packet property.</summary>
/// <param name="Value">The encoded property bytes, retained without copying or validation.</param>
public record VarLongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<VarLongProperty>
{
    /// <summary>Gets the signed 64-bit value decoded from the variable-length bytes.</summary>
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarLong();

    /// <summary>Encodes a signed 64-bit integer using Minecraft variable-length encoding.</summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The encoded property.</returns>
    public static VarLongProperty FromPrimitive(long value)
    {
        Span<byte> bytes = stackalloc byte[10];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteVarLong(value);

        return new VarLongProperty(bytes[..(int)buffer.Position].ToArray());
    }

    /// <summary>Reads one variable-length signed 64-bit integer from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    /// <exception cref="InvalidOperationException">The encoding uses more than ten bytes.</exception>
    public static VarLongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadVarLong());
    }

    /// <summary>Writes the decoded value using Minecraft variable-length encoding.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarLong(AsPrimitive);
    }
}
