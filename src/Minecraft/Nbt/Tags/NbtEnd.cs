using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents the payload-free tag that terminates an NBT compound.</summary>
public record NbtEnd : NbtTag
{
    private static readonly NbtEnd _nbtEnd = new();
    private static readonly EndTag _endTag = new();

    /// <summary>
    /// Converts an <see cref="EndTag"/> to the shared <see cref="NbtEnd"/> instance.
    /// </summary>
    /// <param name="_">The end tag to convert. Its state is not inspected.</param>
    /// <returns>The shared <see cref="NbtEnd"/> instance.</returns>
    public static implicit operator NbtEnd(EndTag _) => _nbtEnd;
    /// <summary>Converts an <see cref="NbtEnd"/> to the shared SharpNBT end-tag instance.</summary>
    /// <param name="_">The end tag; its state is not inspected.</param>
    /// <returns>The shared SharpNBT end tag.</returns>
    public static implicit operator EndTag(NbtEnd _) => _endTag;

    /// <summary>Serializes this end tag as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
    public override string ToString() => ToSnbt();
}
