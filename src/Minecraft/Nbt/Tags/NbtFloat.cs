using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents a single-precision floating-point NBT tag.</summary>
/// <param name="Value">The numeric value.</param>
public record NbtFloat(float Value) : NbtTag
{
    /// <summary>Converts a SharpNBT float tag while preserving its name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtFloat(FloatTag tag) => new(tag.Value) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT float tag while preserving the name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator FloatTag(NbtFloat tag) => new(tag.Name, tag.Value);

    /// <summary>Serializes this tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
