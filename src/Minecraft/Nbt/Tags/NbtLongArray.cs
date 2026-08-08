using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents an NBT signed-long-array tag.</summary>
/// <param name="Data">The array retained by the tag.</param>
public record NbtLongArray(long[] Data) : NbtTag
{
    /// <summary>Converts a SharpNBT long-array tag while preserving its name and data.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtLongArray(LongArrayTag tag) => new((long[])tag) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT long-array tag while preserving the name and data.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator LongArrayTag(NbtLongArray tag) => new(tag.Name, tag.Data);

    /// <summary>
    /// Returns this long array tag serialized as stringified named binary tag (SNBT).
    /// </summary>
    /// <returns>The SNBT representation of the current <see cref="NbtLongArray"/>.</returns>
    public override string ToString() => ToSnbt();
}
