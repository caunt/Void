using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents an NBT string tag.</summary>
/// <param name="Value">The string value.</param>
public record NbtString(string Value) : NbtTag
{
    /// <summary>Converts a SharpNBT string tag while preserving its name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtString(StringTag tag) => new(tag.Value) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT string tag while preserving the name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator StringTag(NbtString tag) => new(tag.Name, tag.Value);

    /// <summary>Serializes this tag as SNBT with quoting and escaping as required.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
