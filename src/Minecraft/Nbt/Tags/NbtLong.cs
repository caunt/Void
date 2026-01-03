namespace Void.Minecraft.Nbt.Tags;

public record NbtLong(long Value) : NbtTag
{
    public static implicit operator NbtLong(LongTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator LongTag(NbtLong tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
