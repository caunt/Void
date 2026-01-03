namespace Void.Minecraft.Nbt.Tags;

public record NbtByteArray(byte[] Data) : NbtTag
{
    public static implicit operator NbtByteArray(ByteArrayTag tag) => new((byte[])tag) { Name = tag.Name };
    public static implicit operator ByteArrayTag(NbtByteArray tag) => new(tag.Name, tag.Data);

    public override string ToString() => ToSnbt();
}
