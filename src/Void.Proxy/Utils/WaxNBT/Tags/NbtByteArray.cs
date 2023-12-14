namespace Void.Proxy.Utils.WaxNBT.Tags;

public class NbtByteArray : NbtTag
{
    public byte[] Data;

    public NbtByteArray(byte[] data) => Data = data;

    public NbtByteArray(string? name, byte[] data)
    {
        Name = name;
        Data = data;
    }

    public static NbtByteArray FromReader(NbtReader reader, bool readName = true)
    {
        string? name = readName ? reader.ReadString() : null;
        int lenght = reader.ReadInt();
        byte[] data = reader.ReadArray(lenght);

        return new NbtByteArray(name, data);
    }

    internal override void SerializeValue(ref NbtWriter writer)
    {
        writer.Write(Data.Length);
        writer.Write(Data);
    }

    public override NbtTagType GetType() => NbtTagType.ByteArray;
}