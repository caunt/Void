using System;
using Void.Minecraft.Nbt.SharpNBT;

namespace Void.Minecraft.Nbt;

/// <summary>Controls byte order and integer encodings used by binary NBT readers and writers.</summary>
[Flags]
public enum NbtFormatOptions
{
    /// <summary>Uses native fixed-width values without an explicit byte-order conversion.</summary>
    None = FormatOptions.None,
    /// <summary>Encodes multi-byte fixed-width values in big-endian order.</summary>
    BigEndian = FormatOptions.BigEndian,
    /// <summary>Encodes multi-byte fixed-width values in little-endian order.</summary>
    LittleEndian = FormatOptions.LittleEndian,
    /// <summary>Encodes eligible integer values as variable-length integers.</summary>
    VarIntegers = FormatOptions.VarIntegers,
    /// <summary>Applies zigzag encoding to signed variable-length integers.</summary>
    ZigZagEncoding = FormatOptions.ZigZagEncoding,
    /// <summary>Uses the big-endian Java Edition NBT format.</summary>
    Java = BigEndian,
    /// <summary>Uses the little-endian Bedrock Edition file format.</summary>
    BedrockFile = LittleEndian,
    /// <summary>Uses the Bedrock Edition network-format option combination.</summary>
    BedrockNetwork = FormatOptions.BedrockNetwork
}
