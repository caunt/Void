using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

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
    public static implicit operator EndTag(NbtEnd _) => _endTag;

    public override string ToString() => ToSnbt();
}
