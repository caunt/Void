namespace Void.NBT.Tags
{
    public class NbtByte : NbtTag
    {
        public byte Value;

        public NbtByte(byte value)
        {
            Value = value;
        }

        public NbtByte(string? name, byte value)
        {
            Name = name;
            Value = value;
        }

        public static NbtByte FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var value = reader.ReadByte();

            return new NbtByte(name, value);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Value);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.Byte;
        }
    }
}