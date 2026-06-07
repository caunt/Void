using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>
/// Represents a double-precision floating-point NBT tag.
/// </summary>
/// <param name="Value">The numeric value stored by the tag.</param>
public record NbtDouble(double Value) : NbtTag
{
    public static implicit operator NbtDouble(DoubleTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator DoubleTag(NbtDouble tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
