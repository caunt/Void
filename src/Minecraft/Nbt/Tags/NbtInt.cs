namespace Void.Minecraft.Nbt.Tags;

public record NbtInt(int Value) : NbtTag
{
    public static implicit operator NbtInt(IntTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator IntTag(NbtInt tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
