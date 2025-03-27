using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtIntArray(int[] Data) : NbtTag
{
    public static implicit operator NbtIntArray(IntArrayTag tag) => new((int[])tag) { Name = tag.Name };
    public static implicit operator IntArrayTag(NbtIntArray tag) => new(tag.Name, tag.Data);

    public override string ToString() => ToSnbt();
}
