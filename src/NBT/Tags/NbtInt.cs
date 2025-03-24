namespace Void.Nbt.Tags
{
    public class NbtInt : NbtTag
    {
        public int Value;

        public NbtInt(int value)
        {
            Value = value;
        }

        public NbtInt(string? name, int value)
        {
            Name = name;
            Value = value;
        }

        public static NbtInt FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var value = reader.ReadInt();

            return new NbtInt(name, value);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Value);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.Int;
        }
    }
}