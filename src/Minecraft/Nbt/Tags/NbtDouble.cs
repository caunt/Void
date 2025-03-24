namespace Void.Minecraft.Nbt.Tags
{
    public class NbtDouble : NbtTag
    {
        public double Value;

        public NbtDouble(double value)
        {
            Value = value;
        }

        public NbtDouble(string? name, double value)
        {
            Name = name;
            Value = value;
        }

        public static NbtDouble FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var value = reader.ReadDouble();

            return new NbtDouble(name, value);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Value);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.Double;
        }
    }
}