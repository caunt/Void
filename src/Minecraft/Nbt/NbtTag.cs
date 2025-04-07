using SharpNBT;
using SharpNBT.SNBT;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json;
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
        NbtBoolean value => (ByteTag)value,
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

    public MemoryStream AsStream(NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        var memoryStream = new MemoryStream();
        var writer = new TagWriter(memoryStream, (FormatOptions)formatOptions);
        var tag = (Tag)this;

        // SharpNBT does not write tag type in case of empty tag name
        if (string.IsNullOrEmpty(Name))
        {
            memoryStream.WriteByte((byte)tag.Type);
            
            // TODO: Handle "UseVarInt" option
            if (Name is not null)
                memoryStream.Write([0, 0]);
        }

        writer.WriteTag(tag);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public JsonNode AsJsonNode()
    {
        return JsonNbtSerializer.Serialize(this);
    }

    public override string ToString() => ToSnbt();

    protected string ToSnbt()
    {
        var tag = (Tag)this;
        return tag.Stringify(!string.IsNullOrEmpty(tag.Name));
    }

    public static NbtCompound ParseCompound(string data)
    {
        return StringNbt.Parse(data);
    }

    public static NbtList ParseList(string data)
    {
        return StringNbt.ParseList(data);
    }

    public static long Parse(ReadOnlyMemory<byte> data, out NbtTag result, bool readName = false, NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        return Parse<NbtTag>(data, out result, readName, formatOptions);
    }

    public static long Parse<T>(ReadOnlyMemory<byte> data, out T result, bool readName = false, NbtFormatOptions formatOptions = NbtFormatOptions.Java) where T : NbtTag
    {
        if (!MemoryMarshal.TryGetArray(data, out var segment) || segment.Array is null)
            throw new Exception("Cannot get array segment from memory");

        var stream = new MemoryStream(segment.Array);
        var reader = new NbtReader(stream, (FormatOptions)formatOptions);
        var tag = (T)reader.ReadTag(readName);

        if (tag is null)
            throw new InvalidCastException($"Tag {tag} cannot be cast to {typeof(T)}");

        result = tag;
        
        if (readName && result.Name is null) 
            result.Name = "";
        
        return stream.Position;
    }
}

public abstract record NbtTag<T> : NbtTag where T : NbtTag
{
    public static long Parse(ReadOnlyMemory<byte> data, out T result, bool readName = false, NbtFormatOptions formatOptions = NbtFormatOptions.Java)
    {
        return Parse<T>(data, out result, readName, formatOptions);
    }
}
