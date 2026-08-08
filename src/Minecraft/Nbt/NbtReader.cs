using System;
using System.IO;
using System.Reflection;
using Void.Minecraft.Nbt.SharpNBT;
using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt;

/// <summary>Reads binary NBT while preserving empty compound keys and supporting unnamed roots.</summary>
/// <param name="stream">The source stream.</param>
/// <param name="options">The binary NBT encoding options.</param>
/// <param name="leaveOpen">Whether to leave <paramref name="stream"/> open when the reader is disposed.</param>
public class NbtReader(Stream stream, FormatOptions options, bool leaveOpen = false) : TagReader(stream, options, leaveOpen)
{
    private static readonly FieldInfo? _tagNameField = typeof(Tag).GetField($"<{nameof(Tag.Name)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>Reads a compound payload through its terminating end tag.</summary>
    /// <param name="named">Whether to read the compound's name before its payload.</param>
    /// <returns>The compound, including children with empty names preserved as empty strings.</returns>
    public new CompoundTag ReadCompound(bool named = true)
    {
        var compoundTag = new CompoundTag(named ? ReadUTF8String() : null);

        while (true)
        {
            var tagType = ReadType();

            if (tagType == TagType.End)
                break;

            var tag = ReadTag(tagType, named: true);

            // TODO: Fix SharpNBT. Name should not be null here because vanilla sometimes sends empty string in key name but SharpNBT changes it to null.
            if (tag.Name is null)
                _tagNameField?.SetValue(tag, "");

            compoundTag.Add(tag);
        }

        return compoundTag;
    }

    /// <summary>Reads a homogeneous list payload.</summary>
    /// <param name="named">Whether to read the list's name before its payload.</param>
    /// <returns>The parsed list.</returns>
    /// <exception cref="FormatException">A non-empty list declares <see cref="TagType.End"/> as its child type.</exception>
    public new ListTag ReadList(bool named = true)
    {
        var name = named ? ReadUTF8String() : null;
        var tagType = ReadType();
        var num = ReadCount();

        if (tagType == TagType.End && num > 0)
            throw new FormatException("An EndTag is not a valid child type for a non-empty ListTag.");

        var listTag = new ListTag(name, tagType);

        while (num-- > 0)
            listTag.Add(ReadTag(tagType, named: false));

        return listTag;
    }

    /// <summary>Reads a tag type identifier followed by its optional name and payload.</summary>
    /// <param name="named">Whether the tag includes a name field.</param>
    /// <returns>The parsed tag.</returns>
    public new Tag ReadTag(bool named = true)
    {
        var type = ReadType();
        return ReadTag(type, named);
    }

    private Tag ReadTag(TagType type, bool named)
    {
        var tag = OnTagEncountered(type, named);

        if (tag is not null)
        {
            OnTagRead(tag);
            return tag;
        }

        var tag2 = type switch
        {
            TagType.End => new EndTag() as Tag,
            TagType.Byte => ReadByte(named),
            TagType.Short => ReadShort(named),
            TagType.Int => ReadInt(named),
            TagType.Long => ReadLong(named),
            TagType.Float => ReadFloat(named),
            TagType.Double => ReadDouble(named),
            TagType.ByteArray => ReadByteArray(named),
            TagType.String => ReadString(named),
            TagType.List => ReadList(named),
            TagType.Compound => ReadCompound(named),
            TagType.IntArray => ReadIntArray(named),
            TagType.LongArray => ReadLongArray(named),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        OnTagRead(tag2);
        return tag2;
    }

    private TagType ReadType()
    {
        var value = BaseStream.ReadByte();

        if (value is -1)
            return TagType.End;

        return (TagType)value;
    }

    private int ReadCount()
    {
        if (!UseVarInt)
            return ReadInt32();

        return VarInt.Read(base.BaseStream, base.ZigZagEncoding);
    }

    private int ReadInt32()
    {
        Span<byte> span = stackalloc byte[4];
        ReadToFixSizedBuffer(span);

        var num = BitConverter.ToInt32(span);

        if (!SwapEndian)
            return num;

        return num.SwapEndian();
    }
}
