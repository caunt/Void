using SharpNBT;
using System;

namespace Void.Minecraft.Nbt.Tags;

public record NbtByte(byte Value = 0) : NbtTag
{
    public static implicit operator NbtByte(ByteTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator ByteTag(NbtByte tag) => new(tag.Name, tag.Value);

    public static implicit operator NbtBoolean(NbtByte tag) => new(Convert.ToBoolean(tag.Value)) { Name = tag.Name };
    public static implicit operator NbtByte(NbtBoolean tag) => (ByteTag)tag;

    public override string ToString() => ToSnbt();
}
