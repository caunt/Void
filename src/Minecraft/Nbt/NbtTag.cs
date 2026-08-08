using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Nbt.Serializers.String;
using Void.Minecraft.Nbt.SharpNBT;
using Void.Minecraft.Nbt.SharpNBT.Snbt;
using Void.Minecraft.Nbt.SharpNBT.Tags;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt;

/// <summary>Provides the common API for tags in the Named Binary Tag data model.</summary>
public abstract record NbtTag
{
    /// <summary>Gets or sets the optional binary tag name.</summary>
    public string? Name { get; set; }
    /// <summary>Gets the NBT payload type.</summary>
    public NbtTagType Type => (NbtTagType)((Tag)this).Type;

    /// <summary>Converts a SharpNBT tag to its Void NBT representation.</summary>
    /// <param name="tag">The SharpNBT tag.</param>
    /// <returns>The corresponding Void tag.</returns>
    /// <exception cref="NotSupportedException">The concrete SharpNBT tag type is unsupported.</exception>
    public static implicit operator NbtTag(Tag tag) => tag switch
    {
        ByteArrayTag value => (NbtByteArray)value,
        ByteTag value => (NbtByte)value,
        CompoundTag value => (NbtCompound)value,
        DoubleTag value => (NbtDouble)value,
        EndTag value => (NbtEnd)value,
        FloatTag value => (NbtFloat)value,
        IntArrayTag value => (NbtIntArray)value,
        IntTag value => (NbtInt)value,
        ListTag value => (NbtList)value,
        LongArrayTag value => (NbtLongArray)value,
        LongTag value => (NbtLong)value,
        ShortTag value => (NbtShort)value,
        StringTag value => (NbtString)value,
        var value => throw new NotSupportedException(value.ToString())
    };

    /// <summary>Converts a Void NBT tag to its SharpNBT representation.</summary>
    /// <param name="tag">The Void tag.</param>
    /// <returns>The corresponding SharpNBT tag.</returns>
    /// <exception cref="NotSupportedException">The concrete Void tag type is unsupported.</exception>
    public static implicit operator Tag(NbtTag tag) => tag switch
    {
        NbtByte value => (ByteTag)value,
        NbtByteArray value => (ByteArrayTag)value,
        NbtCompound value => (CompoundTag)value,
        NbtDouble value => (DoubleTag)value,
        NbtEnd value => (EndTag)value,
        NbtFloat value => (FloatTag)value,
        NbtInt value => (IntTag)value,
        NbtIntArray value => (IntArrayTag)value,
        NbtList value => (ListTag)value,
        NbtLong value => (LongTag)value,
        NbtLongArray value => (LongArrayTag)value,
        NbtShort value => (ShortTag)value,
        NbtString value => (StringTag)value,
        var value => throw new NotSupportedException(value.ToString())
    };

    /// <summary>Serializes this tag to a seekable in-memory binary stream.</summary>
    /// <param name="formatOptions">The byte-order and integer-encoding options.</param>
    /// <param name="writeName">Whether to include the tag-name field.</param>
    /// <returns>A new stream positioned at zero.</returns>
    /// <remarks>When <paramref name="writeName"/> is <see langword="false"/>, <see cref="Name"/> is temporarily cleared and restored after serialization.</remarks>
    public MemoryStream AsStream(NbtFormatOptions formatOptions = NbtFormatOptions.Java, bool writeName = true)
    {
        var stream = new MemoryStream();
        var writer = new TagWriter(stream, (FormatOptions)formatOptions);

        // Remove name to force tag serialization without name
        if (!writeName)
            Name = null;

        var name = Name;
        var tag = (Tag)this;

        // SharpNBT does not write tag type in case of empty tag name
        if (name is null)
        {
            stream.WriteByte((byte)tag.Type);

            if (writeName)
                stream.Write([0, 0]);
        }

        writer.WriteTag(tag);
        stream.Position = 0;

        // Restore name
        if (!writeName)
            Name = name;

        return stream;
    }

    /// <summary>Serializes this tag to the library's typed JSON representation.</summary>
    /// <returns>The JSON node.</returns>
    public JsonNode AsJsonNode()
    {
        return NbtJsonSerializer.Serialize(this);
    }

    /// <summary>Serializes this tag as stringified NBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();

    /// <summary>Serializes this tag as stringified NBT for derived tag implementations.</summary>
    /// <returns>The SNBT representation.</returns>
    protected string ToSnbt() => NbtStringSerializer.Serialize(this);

    /// <summary>Parses an SNBT compound or list.</summary>
    /// <param name="data">The SNBT text.</param>
    /// <returns>The parsed compound or list tag.</returns>
    /// <exception cref="FormatException">The trimmed input does not begin with <c>{</c> or <c>[</c>, or contains invalid SNBT.</exception>
    public static NbtTag Parse(string data)
    {
        ReadOnlySpan<char> span = data.AsSpan().TrimStart();

        if (!span.IsEmpty && span[0] == '{')
            return StringNbt.Parse(data);
        else if (!span.IsEmpty && span[0] == '[')
            return StringNbt.ParseList(data);
        else
            throw new FormatException($"Only NbtCompound and NbtList can be parsed from Snbt. Provided value: {data}");
    }

    /// <summary>Parses one binary NBT tag.</summary>
    /// <param name="data">Array-backed binary NBT data.</param>
    /// <param name="result">The parsed tag.</param>
    /// <param name="readName">Whether the root tag includes a name field.</param>
    /// <param name="formatOptions">The byte-order and integer-encoding options.</param>
    /// <returns>The stream position after the parsed tag.</returns>
    /// <exception cref="ArgumentException"><paramref name="data"/> is not backed by an accessible array.</exception>
    public static long Parse(ReadOnlyMemory<byte> data, out NbtTag result, bool readName = true, NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        return Parse<NbtTag>(data, out result, readName, formatOptions);
    }

    /// <summary>Parses one binary NBT tag and requires a specific tag type.</summary>
    /// <typeparam name="T">The required tag type.</typeparam>
    /// <param name="data">Array-backed binary NBT data.</param>
    /// <param name="result">The parsed tag.</param>
    /// <param name="readName">Whether the root tag includes a name field.</param>
    /// <param name="formatOptions">The byte-order and integer-encoding options.</param>
    /// <returns>The stream position after the parsed tag.</returns>
    /// <exception cref="ArgumentException"><paramref name="data"/> is not backed by an accessible array.</exception>
    /// <exception cref="InvalidCastException">The parsed tag is not assignable to <typeparamref name="T"/>.</exception>
    public static long Parse<T>(ReadOnlyMemory<byte> data, out T result, bool readName = true, NbtFormatOptions formatOptions = NbtFormatOptions.Java) where T : NbtTag
    {
        if (!MemoryMarshal.TryGetArray(data, out var segment) || segment.Array is null)
            throw new ArgumentException("Cannot get array segment from data", nameof(data));

        using var stream = new MemoryStream(segment.Array);
        var reader = new NbtReader(stream, (FormatOptions)formatOptions);
        var tag = (T)reader.ReadTag(readName);

        if (tag is null)
            throw new InvalidCastException($"Tag {tag} cannot be cast to {typeof(T)}");

        result = tag;

        if (readName && result.Name is null)
            result.Name = "";

        return stream.Position;
    }

    /// <summary>Reads one binary NBT tag from the current buffer position.</summary>
    /// <typeparam name="TBuffer">The Minecraft buffer type.</typeparam>
    /// <param name="buffer">The buffer to consume.</param>
    /// <param name="readName">Whether the root tag includes a name field.</param>
    /// <returns>The parsed tag.</returns>
    /// <remarks>The implementation reads the remaining buffer data, then advances the original buffer only by the parsed tag length.</remarks>
    public static NbtTag ReadFrom<TBuffer>(ref TBuffer buffer, bool readName = true) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var position = buffer.Position;
        var data = buffer.ReadToEnd();
        var length = (int)Parse(data.ToArray(), out var result, readName); // TODO: Allocation, use buffer directly

        buffer.Position = position + length;
        return result;
    }
}

/// <summary>Provides typed binary parsing for an NBT tag family.</summary>
/// <typeparam name="T">The concrete tag type returned by parsing.</typeparam>
public abstract record NbtTag<T> : NbtTag where T : NbtTag
{
    /// <summary>Parses one binary NBT tag of type <typeparamref name="T"/>.</summary>
    /// <param name="data">Array-backed binary NBT data.</param>
    /// <param name="result">The parsed tag.</param>
    /// <param name="readName">Whether the root tag includes a name field.</param>
    /// <param name="formatOptions">The byte-order and integer-encoding options.</param>
    /// <returns>The stream position after the parsed tag.</returns>
    public static long Parse(ReadOnlyMemory<byte> data, out T result, bool readName = true, NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        return Parse<T>(data, out result, readName, formatOptions);
    }
}
