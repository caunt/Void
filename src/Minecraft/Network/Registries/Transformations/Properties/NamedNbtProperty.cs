using System;
using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents a named binary NBT packet property.
/// </summary>
/// <param name="Value">The encoded named NBT bytes, retained without copying or validation.</param>
public record NamedNbtProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<NamedNbtProperty>
{
    /// <summary>
    /// Gets a newly parsed NBT tag from the encoded bytes, including its root name.
    /// </summary>
    public NbtTag AsNbtTag => new MinecraftBuffer(Value.Span).ReadTag(readName: true);

    /// <summary>
    /// Serializes a tag with its root name.
    /// </summary>
    /// <remarks>If <paramref name="value" /> has a null name, this method mutates it by assigning an empty name before serialization.</remarks>
    /// <param name="value">The tag to serialize.</param>
    /// <returns>A property backed by the serialized bytes.</returns>
    public static NamedNbtProperty FromNbtTag(NbtTag value)
    {
        value.Name ??= string.Empty;

        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteTag(value, writeName: true);

        return new NamedNbtProperty(stream.GetBuffer().AsMemory(0, (int)stream.Length));
    }

    /// <summary>
    /// Reads one named NBT tag and stores its normalized binary representation.
    /// </summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static NamedNbtProperty Read(ref MinecraftBuffer buffer)
    {
        return FromNbtTag(buffer.ReadTag(readName: true));
    }

    /// <summary>
    /// Parses and writes the NBT value with its root name.
    /// </summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteTag(AsNbtTag);
    }
}
