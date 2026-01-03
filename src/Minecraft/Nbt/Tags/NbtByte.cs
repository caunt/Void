using System;

namespace Void.Minecraft.Nbt.Tags;

public record NbtByte(byte Value = 0) : NbtTag
{
    public bool IsBool => Value < 2;
    public bool IsTrue => Value is 1;
    public bool IsFalse => Value is 0;

    public NbtByte(bool value) : this(Convert.ToByte(value))
    {
        // Empty
    }

    public static implicit operator NbtByte(ByteTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator ByteTag(NbtByte tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
