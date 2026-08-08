using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents a signed 32-bit integer NBT tag.</summary>
/// <param name="Value">The numeric value.</param>
public record NbtInt(int Value) : NbtTag
{
    /// <summary>Converts a SharpNBT integer tag while preserving its name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtInt(IntTag tag) => new(tag.Value) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT integer tag while preserving the name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator IntTag(NbtInt tag) => new(tag.Name, tag.Value);

    /// <summary>Serializes this tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
