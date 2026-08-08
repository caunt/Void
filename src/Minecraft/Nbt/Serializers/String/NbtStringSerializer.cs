using Void.Minecraft.Nbt.SharpNBT.Snbt;
using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Serializers.String;

/// <summary>Converts NBT tags to and from standard stringified NBT.</summary>
public static class NbtStringSerializer
{
    /// <summary>Serializes an NBT tag, including its name only when the name is nonempty.</summary>
    /// <param name="tag">The tag to serialize.</param>
    /// <returns>The SNBT representation.</returns>
    public static string Serialize(NbtTag tag)
    {
        var sharpNbtTag = (Tag)tag;
        return sharpNbtTag.Stringify(!string.IsNullOrEmpty(sharpNbtTag.Name));
    }

    /// <summary>Parses an SNBT compound.</summary>
    /// <param name="value">The SNBT text.</param>
    /// <returns>The parsed tag.</returns>
    public static NbtTag Deserialize(string value)
    {
        return StringNbt.Parse(value);
    }
}
