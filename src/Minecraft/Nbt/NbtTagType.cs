using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt;

/// <summary>Identifies the payload type encoded by an NBT tag.</summary>
public enum NbtTagType : byte
{
    /// <summary>Marks the end of a compound and has no payload.</summary>
    End = TagType.End,
    /// <summary>A signed 8-bit integer.</summary>
    Byte = TagType.Byte,
    /// <summary>A signed 16-bit integer.</summary>
    Short = TagType.Short,
    /// <summary>A signed 32-bit integer.</summary>
    Int = TagType.Int,
    /// <summary>A signed 64-bit integer.</summary>
    Long = TagType.Long,
    /// <summary>A 32-bit floating-point number.</summary>
    Float = TagType.Float,
    /// <summary>A 64-bit floating-point number.</summary>
    Double = TagType.Double,
    /// <summary>An array of signed 8-bit integers.</summary>
    ByteArray = TagType.ByteArray,
    /// <summary>A modified UTF-8 string.</summary>
    String = TagType.String,
    /// <summary>A homogeneous ordered tag sequence.</summary>
    List = TagType.List,
    /// <summary>A keyed tag collection.</summary>
    Compound = TagType.Compound,
    /// <summary>An array of signed 32-bit integers.</summary>
    IntArray = TagType.IntArray,
    /// <summary>An array of signed 64-bit integers.</summary>
    LongArray = TagType.LongArray
}
