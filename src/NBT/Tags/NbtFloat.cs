namespace Void.Nbt.Tags
{
    public class NbtFloat : NbtTag
    {
        public float Value;

        public NbtFloat(float value)
        {
            Value = value;
        }

        public NbtFloat(string? name, float value)
        {
            Name = name;
            Value = value;
        }

        public static NbtFloat FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var value = reader.ReadFloat();

            return new NbtFloat(name, value);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Value);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.Float;
        }
    }
}