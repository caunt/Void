using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtFloat(float Value) : NbtTag
{
    public static implicit operator NbtFloat(FloatTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator FloatTag(NbtFloat tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
