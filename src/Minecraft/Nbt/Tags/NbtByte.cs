using System;
using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>Represents an 8-bit NBT numeric tag, including the conventional boolean values zero and one.</summary>
/// <param name="Value">The stored byte.</param>
public record NbtByte(byte Value = 0) : NbtTag
{
    /// <summary>Gets whether the value is zero or one and can therefore represent an NBT boolean.</summary>
    public bool IsBool => Value < 2;
    /// <summary>Gets whether the value is one.</summary>
    public bool IsTrue => Value is 1;
    /// <summary>Gets whether the value is zero.</summary>
    public bool IsFalse => Value is 0;

    /// <summary>Creates a byte tag containing one for <see langword="true"/> or zero for <see langword="false"/>.</summary>
    /// <param name="value">The boolean value.</param>
    public NbtByte(bool value) : this(Convert.ToByte(value))
    {
        // Empty
    }

    /// <summary>Converts a SharpNBT byte tag while preserving its name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator NbtByte(ByteTag tag) => new(tag.Value) { Name = tag.Name };
    /// <summary>Converts to a SharpNBT byte tag while preserving the name and value.</summary>
    /// <param name="tag">The source tag.</param>
    /// <returns>The converted tag.</returns>
    public static implicit operator ByteTag(NbtByte tag) => new(tag.Name, tag.Value);

    /// <summary>
    /// Returns the byte tag serialized in stringified NBT (SNBT) format.
    /// </summary>
    /// <returns>The SNBT representation of this byte tag.</returns>
    public override string ToString() => ToSnbt();
}
