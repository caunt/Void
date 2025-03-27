using SharpNBT;

namespace Void.Minecraft.Nbt;

public enum NbtTagType : byte
{
    End = TagType.End,
    Byte = TagType.Byte,
    Short = TagType.Short,
    Int = TagType.Int,
    Long = TagType.Long,
    Float = TagType.Float,
    Double = TagType.Double,
    ByteArray = TagType.ByteArray,
    String = TagType.String,
    List = TagType.List,
    Compound = TagType.Compound,
    IntArray = TagType.IntArray,
    LongArray = TagType.LongArray
}
