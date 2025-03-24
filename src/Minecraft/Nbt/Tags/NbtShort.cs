namespace Void.Minecraft.Nbt.Tags
{
    public class NbtShort : NbtTag
    {
        public short Value;

        public NbtShort(short value)
        {
            Value = value;
        }

        public NbtShort(string? name, short value)
        {
            Name = name;
            Value = value;
        }

        public static NbtShort FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var value = reader.ReadShort();

            return new NbtShort(name, value);
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Value);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.Short;
        }
    }
}