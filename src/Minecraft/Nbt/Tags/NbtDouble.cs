using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>
/// Represents a double-precision floating-point NBT tag.
/// </summary>
/// <param name="Value">The numeric value stored by the tag.</param>
public record NbtDouble(double Value) : NbtTag
{
    /// <summary>Converts a SharpNBT double tag while preserving its name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtDouble(DoubleTag tag) => new(tag.Value) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT double tag while preserving the name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator DoubleTag(NbtDouble tag) => new(tag.Name, tag.Value);

    /// <summary>Serializes this tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
