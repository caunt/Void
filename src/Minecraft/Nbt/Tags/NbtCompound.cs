using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpNBT;

namespace Void.Minecraft.Nbt.Tags;

public record NbtCompound(Dictionary<string, NbtTag> Fields) : NbtTag
{
    public NbtCompound() : this([])
    {

    }

    public NbtTag? this[string name]
    {
        get
        {
            if (!Fields.TryGetValue(name, out var value))
                return null;

            return value;
        }
        set
        {
            value ??= new NbtCompound();
            value.Name = name;
            Fields[name] = value;
        }
    }

    public static implicit operator NbtCompound(CompoundTag tag) => new(tag.Values.ToDictionary(pair => pair.Name ?? string.Empty, pair => (NbtTag)pair)) { Name = tag.Name };
    public static implicit operator CompoundTag(NbtCompound tag) => new(tag.Name, tag.Fields?.Select(pair => (Tag)pair.Value) ?? []);

    public bool ContainsKey(string name) => Fields.ContainsKey(name);
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out NbtTag value) => Fields.TryGetValue(name, out value);
    public override string ToString() => ToSnbt();
}
