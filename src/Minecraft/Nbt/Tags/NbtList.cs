using System.Collections.Generic;
using System.Linq;

namespace Void.Minecraft.Nbt.Tags;

public record NbtList(IEnumerable<NbtTag> Data, NbtTagType DataType) : NbtTag
{
    public static implicit operator NbtList(ListTag tag) => new(tag.Select(tag => (NbtTag)tag), (NbtTagType)tag.ChildType) { Name = tag.Name };
    public static implicit operator ListTag(NbtList tag) => new(tag.Name, (TagType)tag.DataType, tag.Data.Select(tag => (Tag)tag));

    public override string ToString() => ToSnbt();
}
