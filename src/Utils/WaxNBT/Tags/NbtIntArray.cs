namespace MinecraftProxy.Utils.WaxNBT.Tags;

public class NbtIntArray : NbtTag
{
    public int[] Data;

    public NbtIntArray(int[] data) => Data = data;

    public NbtIntArray(string? name, int[] data)
    {
        Name = name;
        Data = data;
    }

    public static NbtIntArray FromReader(NbtReader reader, bool readName = true)
    {
        string? name = readName ? reader.ReadString() : null;
        int lenght = reader.ReadInt();

        int[] data = new int[lenght];

        for (int i = 0; i < lenght; i++)
            data[i] = reader.ReadInt();

        return new NbtIntArray(name, data);
    }

    internal override void SerializeValue(ref NbtWriter writer)
    {
        writer.Write(Data.Length);
        foreach (int i in Data) writer.Write(i);
    }

    public override NbtTagType GetType() => NbtTagType.IntArray;
}