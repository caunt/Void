namespace Void.NBT.Tags
{
    public class NbtEnd : NbtTag
    {
        internal override void SerializeValue(ref NbtWriter writer) { }

        public static NbtEnd FromReader() => new NbtEnd();

        public override NbtTagType GetType() => NbtTagType.End;
    }
}