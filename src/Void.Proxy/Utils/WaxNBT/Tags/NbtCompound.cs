namespace Void.Proxy.Utils.WaxNBT.Tags;

public class NbtCompound : NbtTag
{
    public List<NbtTag> Children = new();

    public NbtCompound(string? name) => Name = name;

    public void Add(NbtTag tag) => Children.Add(tag);

    public static NbtCompound FromReader(NbtReader reader, bool readName = true)
    {
        string? name = readName ? reader.ReadString() : null;

        NbtCompound compound = new NbtCompound(name);
        NbtTag tag = reader.ReadTag();

        while (tag.GetType() != NbtTagType.End)
        {
            compound.Add(tag);
            tag = reader.ReadTag();
        }

        return compound;
    }

    internal override void SerializeValue(ref NbtWriter writer)
    {
        foreach (NbtTag child in Children) child.Serialize(ref writer);
        writer.Write(NbtTagType.End);
    }

    public override NbtTagType GetType() => NbtTagType.Compound;
}