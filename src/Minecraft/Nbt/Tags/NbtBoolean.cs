using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtBoolean(bool Value = false) : NbtTag
{
    public static implicit operator ByteTag(NbtBoolean tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
