using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents an NBT byte-array tag.</summary>
/// <param name="Data">The array retained by the tag.</param>
public record NbtByteArray(byte[] Data) : NbtTag
{
    /// <summary>Converts a SharpNBT byte-array tag while preserving its name and data.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtByteArray(ByteArrayTag tag) => new((byte[])tag) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT byte-array tag while preserving the name and data.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator ByteArrayTag(NbtByteArray tag) => new(tag.Name, tag.Data);

    /// <summary>Serializes this tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
