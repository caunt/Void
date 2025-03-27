using SharpNBT;
using System.Collections.Generic;
using System.Linq;

namespace Void.Minecraft.Nbt.Tags;

public record NbtCompound(Dictionary<string, NbtTag> Values) : NbtTag
{
    public NbtCompound() : this([])
    {

    }

    public NbtTag this[string name]
    {
        get
        {
            return Values[name];
        }
        set
        {
            value.Name = name;
            Values[name] = value;
        }
    }

    public static implicit operator NbtCompound(CompoundTag tag) => new(tag.Values.ToDictionary(pair => pair.Name ?? string.Empty, pair => (NbtTag)pair)) { Name = tag.Name };
    public static implicit operator CompoundTag(NbtCompound tag) => new(tag.Name, tag.Values?.Select(pair => (Tag)pair.Value) ?? []);

    public override string ToString() => ToSnbt();
}
