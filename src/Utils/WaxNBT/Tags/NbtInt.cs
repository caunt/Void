namespace MinecraftProxy.Utils.WaxNBT.Tags;

public class NbtInt : NbtTag
{
    public int Value;

    public NbtInt(int value) => Value = value;

    public NbtInt(string? name, int value)
    {
        Name = name;
        Value = value;
    }

    public static NbtInt FromReader(NbtReader reader, bool readName = true)
    {
        string? name = readName ? reader.ReadString() : null;
        int value = reader.ReadInt();

        return new NbtInt(name, value);
    }

    internal override void SerializeValue(ref NbtWriter writer) => writer.Write(Value);

    public override NbtTagType GetType() => NbtTagType.Int;
}