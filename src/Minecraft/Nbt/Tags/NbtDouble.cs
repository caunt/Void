using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtDouble(double Value) : NbtTag
{
    public static implicit operator NbtDouble(DoubleTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator DoubleTag(NbtDouble tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
