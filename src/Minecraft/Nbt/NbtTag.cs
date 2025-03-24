namespace Void.Minecraft.Nbt
{
    public abstract class NbtTag
    {
        public string? Name = null;

        internal void Serialize(ref NbtWriter writer)
        {
            WriteHeader(ref writer);
            SerializeValue(ref writer);
        }

        internal abstract void SerializeValue(ref NbtWriter writer);

        protected void WriteHeader(ref NbtWriter writer)
        {
            writer.Write(GetType());

            if (Name != null)
                writer.Write(Name);
        }

        public new abstract NbtTagType GetType();
    }
}