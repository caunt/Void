using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtShort(short Value) : NbtTag
{
    public static implicit operator NbtShort(ShortTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator ShortTag(NbtShort tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
