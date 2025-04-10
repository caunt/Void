using SharpNBT;
using System;

namespace Void.Minecraft.Nbt.Tags;

public record NbtBoolean(bool Value = false) : NbtTag
{
    public static implicit operator NbtBoolean(ByteTag tag) => new(Convert.ToBoolean(tag.Value)) { Name = tag.Name };
    public static implicit operator ByteTag(NbtBoolean tag) => new(tag.Name, tag.Value);

    public static implicit operator NbtBoolean(NbtByte tag) => new(Convert.ToBoolean(tag.Value)) { Name = tag.Name };
    public static implicit operator NbtByte(NbtBoolean tag) => new(Convert.ToByte(tag.Value)) { Name = tag.Name };

    public override string ToString() => ToSnbt();
}
