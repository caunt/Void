using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Nbt.Serializers.String;
using Void.Minecraft.Nbt.Snbt;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt;

public abstract record NbtTag
{
    public string? Name { get; set; }
    public NbtTagType Type => (NbtTagType)((Tag)this).Type;

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

    public MemoryStream AsStream(NbtFormatOptions formatOptions = NbtFormatOptions.Java, bool writeName = true)
    {
        var stream = new MemoryStream();
        var writer = new TagWriter(stream, (FormatOptions)formatOptions);
        var name = Name;

        // Remove name to force tag serialization without name
        if (!writeName)
            Name = null;

        var tag = (Tag)this;

        stream.WriteByte((byte)tag.Type);

        // Void.Minecraft.Nbt does not write tag type in case of empty tag name
        if (writeName && string.IsNullOrEmpty(name))
        {
            // TODO: Handle "UseVarInt" option
            if (name is not null)
                stream.Write([0, 0]);
        }

        writer.WriteTag(tag);
        stream.Position = 0;

        // Restore name
        if (!writeName)
            Name = name;

        return stream;
    }

    public JsonNode AsJsonNode()
    {
        return NbtJsonSerializer.Serialize(this);
    }

    public override string ToString() => ToSnbt();

    protected string ToSnbt() => NbtStringSerializer.Serialize(this);

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

    public static long Parse(ReadOnlyMemory<byte> data, out NbtTag result, bool readName = false, NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        return Parse<NbtTag>(data, out result, readName, formatOptions);
    }

    public static long Parse<T>(ReadOnlyMemory<byte> data, out T result, bool readName = false, NbtFormatOptions formatOptions = NbtFormatOptions.Java) where T : NbtTag
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

    public static NbtTag ReadFrom<TBuffer>(ref TBuffer buffer, bool readName = false) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var position = buffer.Position;
        var data = buffer.ReadToEnd();
        var length = (int)Parse(data.ToArray(), out var result, readName); // TODO: Allocation, use buffer directly

        buffer.Position = position + length;
        return result;
    }
}

public abstract record NbtTag<T> : NbtTag where T : NbtTag
{
    public static long Parse(ReadOnlyMemory<byte> data, out T result, bool readName = false, NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        return Parse<T>(data, out result, readName, formatOptions);
    }
}
