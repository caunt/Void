using SharpNBT.SNBT;

namespace Void.Minecraft.Nbt.Serializers.String;

public static class NbtStringSerializer
{
    public static string Serialize(NbtTag tag)
    {
        return tag.ToString();
    }

    public static NbtTag Deserialize(string value)
    {
        return StringNbt.Parse(value);
    }
}
