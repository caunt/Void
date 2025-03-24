namespace Void.Minecraft.Nbt.Tags
{
    public class NbtEnd : NbtTag
    {
        internal override void SerializeValue(ref NbtWriter writer)
        {
        }

        public static NbtEnd FromReader()
        {
            return new NbtEnd();
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.End;
        }
    }
}