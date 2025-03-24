namespace Void.Minecraft.Nbt.Tags
{
    public class NbtByteArray : NbtTag
    {
        public byte[] Data;

        public NbtByteArray(byte[] data)
        {
            Data = data;
        }

        public NbtByteArray(string? name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public static NbtByteArray FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var length = reader.ReadInt();
            var data = reader.ReadArray(length);

            return new NbtByteArray(name, data);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Data.Length);
            writer.Write(Data);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.ByteArray;
        }
    }
}