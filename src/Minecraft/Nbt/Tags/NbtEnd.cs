namespace Void.Minecraft.Nbt.Tags;

public record NbtEnd : NbtTag
{
    private static readonly NbtEnd _nbtEnd = new();
    private static readonly EndTag _endTag = new();

    public static implicit operator NbtEnd(EndTag _) => _nbtEnd;
    public static implicit operator EndTag(NbtEnd _) => _endTag;

    public override string ToString() => ToSnbt();
}
