using Void.Minecraft.Nbt.Snbt;

namespace Void.Minecraft.Nbt.Serializers.String;

public static class NbtStringSerializer
{
    public static string Serialize(NbtTag tag)
    {
        var sharpNbtTag = (Tag)tag;
        return sharpNbtTag.Stringify(!string.IsNullOrEmpty(sharpNbtTag.Name));
    }

    public static NbtTag Deserialize(string value)
    {
        return StringNbt.Parse(value);
    }
}
