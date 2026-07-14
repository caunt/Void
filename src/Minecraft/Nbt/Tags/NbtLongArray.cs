using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

public record NbtLongArray(long[] Data) : NbtTag
{
    public static implicit operator NbtLongArray(LongArrayTag tag) => new((long[])tag) { Name = tag.Name };
    public static implicit operator LongArrayTag(NbtLongArray tag) => new(tag.Name, tag.Data);

    /// <summary>
    /// Returns this long array tag serialized as stringified named binary tag (SNBT).
    /// </summary>
    /// <returns>The SNBT representation of the current <see cref="NbtLongArray"/>.</returns>
    public override string ToString() => ToSnbt();
}
