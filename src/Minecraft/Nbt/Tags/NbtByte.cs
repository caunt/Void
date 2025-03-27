using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtByte(byte Value = 0) : NbtTag
{
    public static implicit operator NbtByte(ByteTag tag) => new(tag.Value) { Name = tag.Name };
    public static implicit operator ByteTag(NbtByte tag) => new(tag.Name, tag.Value);

    public override string ToString() => ToSnbt();
}
