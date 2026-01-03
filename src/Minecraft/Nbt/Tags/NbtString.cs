namespace Void.Minecraft.Nbt.Tags;

public record NbtString(string Value) : NbtTag
{
    public static implicit operator NbtString(StringTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator StringTag(NbtString tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
