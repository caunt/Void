using System;
using SharpNBT;

namespace Void.Minecraft.Nbt;

[Flags]
public enum NbtFormatOptions
{
    None = FormatOptions.None,
    BigEndian = FormatOptions.BigEndian,
    LittleEndian = FormatOptions.LittleEndian,
    VarIntegers = FormatOptions.VarIntegers,
    ZigZagEncoding = FormatOptions.ZigZagEncoding,
    Java = BigEndian,
    BedrockFile = LittleEndian,
    BedrockNetwork = FormatOptions.BedrockNetwork
}
