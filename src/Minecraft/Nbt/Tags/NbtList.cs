using System.Collections.Generic;

namespace Void.Minecraft.Nbt.Tags
{
    public class NbtList : NbtTag
    {
        public List<NbtTag> Data = new List<NbtTag>();
        public NbtTagType Type;

        public NbtList(string? name, NbtTagType type)
        {
            (Name, Type) = (name, type);
        }

        public static NbtList FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;
            var type = reader.ReadTagType();
            var count = reader.ReadInt();

            var list = new NbtList(name, type);

            for (var i = 0; i < count; i++)
                list.Data.Add(reader.ReadTag(type, false));

            return list;
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            writer.Write(Type);
            writer.Write(Data.Count);

            foreach (var tag in Data)
                tag.SerializeValue(ref writer);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.List;
        }
    }
}