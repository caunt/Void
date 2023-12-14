namespace Void.Proxy.Utils.WaxNBT.Tags;

public class NbtLong : NbtTag
{
    public long Value;

    public NbtLong(long value) => Value = value;

    public NbtLong(string? name, long value)
    {
        Name = name;
        Value = value;
    }

    public static NbtLong FromReader(NbtReader reader, bool readName = true)
    {
        string? name = readName ? reader.ReadString() : null;
        long value = reader.ReadLong();

        return new NbtLong(name, value);
    }

    internal override void SerializeValue(ref NbtWriter writer) => writer.Write(Value);

    public override NbtTagType GetType() => NbtTagType.Long;
}