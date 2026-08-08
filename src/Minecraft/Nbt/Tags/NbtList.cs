using System.Collections.Generic;
using System.Linq;
using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents a homogeneous ordered NBT tag sequence.</summary>
/// <param name="Data">The child-tag sequence.</param>
/// <param name="DataType">The declared type shared by the children.</param>
public record NbtList(IEnumerable<NbtTag> Data, NbtTagType DataType) : NbtTag
{
    /// <summary>Converts a SharpNBT list while preserving its name, child type, and child sequence.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted list.</returns>
    public static implicit operator NbtList(ListTag tag) => new(tag.Select(tag => (NbtTag)tag), (NbtTagType)tag.ChildType) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT list while preserving the name, child type, and child sequence.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted list.</returns>
    public static implicit operator ListTag(NbtList tag) => new(tag.Name, (TagType)tag.DataType, tag.Data.Select(tag => (Tag)tag));

    /// <summary>Serializes this list as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
