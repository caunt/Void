namespace Void.Minecraft.Nbt.Tags
{
    public class NbtIntArray : NbtTag
    {
        public int[] Data;

        public NbtIntArray(int[] data)
        {
            Data = data;
        }

        public NbtIntArray(string? name, int[] data)
        {
            Name = name;
            Data = data;
        }

        public static NbtIntArray FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var length = reader.ReadInt();

            var data = new int[length];

            for (var i = 0; i < length; i++)
                data[i] = reader.ReadInt();

            return new NbtIntArray(name, data);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Data.Length);

            foreach (var i in Data)
                writer.Write(i);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.IntArray;
        }
    }
}