using System.Collections.Generic;
using System.Linq;

namespace Void.Nbt.Tags
{
    public class NbtCompound : NbtTag
    {
        public List<NbtTag> Children = new List<NbtTag>();

        public NbtTag this[string name] => Children.First(tag => tag.Name == name);

        public NbtCompound(string? name)
        {
            Name = name;
        }

        public void Add(NbtTag tag)
        {
            Children.Add(tag);
        }

        public static NbtCompound FromReader(NbtReader reader, bool readName = true)
        {
            var name = readName ? reader.ReadString() : null;

            var compound = new NbtCompound(name);
            var tag = reader.ReadTag();

            while (tag.GetType() != NbtTagType.End)
            {
                compound.Add(tag);
                tag = reader.ReadTag();
            }

            return compound;
        }

        internal override void SerializeValue(ref NbtWriter writer)
        {
            foreach (var child in Children)
                child.Serialize(ref writer);

            writer.Write(NbtTagType.End);
        }

        public override NbtTagType GetType()
        {
            return NbtTagType.Compound;
        }
    }
}