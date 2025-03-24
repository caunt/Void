namespace Void.Nbt.Tags
{
    public class NbtString : NbtTag
    {
        public string Value;

        public NbtString(string value)
        {
            Value = value;
        }

        public NbtString(string? name, string value)
        {
            Name = name;
            Value = value;
        }

        public static NbtString FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var value = reader.ReadString();

            return new NbtString(name, value);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Value);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.String;
        }
    }
}