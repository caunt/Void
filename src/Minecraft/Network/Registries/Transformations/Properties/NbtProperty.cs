using System;
using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents an unnamed binary NBT packet property.
/// </summary>
/// <param name="Value">The encoded unnamed NBT bytes, retained without copying or validation.</param>
public record NbtProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<NbtProperty>
{
    /// <summary>
    /// Gets a newly parsed NBT tag from the encoded bytes without reading a root name.
    /// </summary>
    public NbtTag AsNbtTag => new MinecraftBuffer(Value.Span).ReadTag(readName: false);

    /// <summary>
    /// Serializes a tag without its root name.
    /// </summary>
    /// <param name="value">The tag to serialize.</param>
    /// <returns>A property backed by the serialized bytes.</returns>
    public static NbtProperty FromNbtTag(NbtTag value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteTag(value, writeName: false);

        return new NbtProperty(stream.GetBuffer().AsMemory(0, (int)stream.Length));
    }

    /// <summary>
    /// Reads one unnamed NBT tag and stores its normalized binary representation.
    /// </summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static NbtProperty Read(ref MinecraftBuffer buffer)
    {
        return FromNbtTag(buffer.ReadTag(readName: false));
    }

    /// <summary>
    /// Parses and writes the NBT value without a root name.
    /// </summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteTag(AsNbtTag, writeName: false);
    }
}
