using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtLongArray(long[] Data) : NbtTag
{
    public static implicit operator NbtLongArray(LongArrayTag tag) => new((long[])tag) { Name = tag.Name };
    public static implicit operator LongArrayTag(NbtLongArray tag) => new(tag.Name, tag.Data);

    public override string ToString() => ToSnbt();
}
