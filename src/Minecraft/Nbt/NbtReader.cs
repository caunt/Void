using System;
using System.IO;
using System.Reflection;
using SharpNBT;

namespace Void.Minecraft.Nbt;

public class NbtReader(Stream stream, FormatOptions options, bool leaveOpen = false) : TagReader(stream, options, leaveOpen)
{
    private static readonly FieldInfo? _tagNameField = typeof(Tag).GetField($"<{nameof(Tag.Name)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public new CompoundTag ReadCompound(bool named = true)
    {
        var compoundTag = new CompoundTag(named ? ReadUTF8String() : null);

        while (true)
        {
            var tagType = ReadType();

            if (tagType == TagType.End)
                break;

            var tag = ReadTag(tagType, named: true);

            // 1.21.4 send empty string tag name with empty value for achievements
            // 1.21.5 send new line characters in chat with empty tag name
            if (string.IsNullOrWhiteSpace(tag.Name))
                _tagNameField?.SetValue(tag, "text");

            compoundTag.Add(tag);
        }

        return compoundTag;
    }

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
