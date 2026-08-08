using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents a signed 64-bit integer NBT tag.</summary>
/// <param name="Value">The numeric value.</param>
public record NbtLong(long Value) : NbtTag
{
    /// <summary>Converts a SharpNBT long tag while preserving its name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtLong(LongTag tag) => new(tag.Value) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT long tag while preserving the name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator LongTag(NbtLong tag) => new(tag.Name, tag.Value);

    /// <summary>Serializes this tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
