using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents an NBT signed-integer-array tag.</summary>
/// <param name="Data">The array retained by the tag.</param>
public record NbtIntArray(int[] Data) : NbtTag
{
    /// <summary>Converts a SharpNBT integer-array tag while preserving its name and data.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtIntArray(IntArrayTag tag) => new((int[])tag) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT integer-array tag while preserving the name and data.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator IntArrayTag(NbtIntArray tag) => new(tag.Name, tag.Data);

    /// <summary>Serializes this tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
