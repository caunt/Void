using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
            // TODO: Consider throwing KeyNotFoundException instead of returning null

            if (!Fields.TryGetValue(name, out var value))
                return null;

            return value;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!TrySetValue(name, value))
                throw new InvalidOperationException($"The same {nameof(NbtTag)} instance cannot be added to a compound more than once. Use {nameof(RenameKey)} method if you intended to rename field.");
        }
    }

    public static implicit operator NbtCompound(CompoundTag tag) => new(tag.Values.ToDictionary(pair => pair.Name ?? string.Empty, pair => (NbtTag)pair)) { Name = tag.Name };
    public static implicit operator CompoundTag(NbtCompound tag) => new(tag.Name, tag.Fields.Select(pair => (Tag)pair.Value));

    public void RenameKey(string name, string newName)
    {
        if (TryRenameKey(name, newName))
            return;

        throw new KeyNotFoundException($"Failed to rename key '{name}' to '{newName}'.");
    }

    public bool TryRenameKey(string name, string newName) => Fields.TryGetValue(name, out var value) && Fields.Remove(name) && TrySetValue(newName, value);
    public bool ContainsKey(string name) => Fields.ContainsKey(name);
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out NbtTag value) => Fields.TryGetValue(name, out value);
    public override string ToString() => ToSnbt();

    private bool TrySetValue(string name, NbtTag value)
    {
        value.Name = name;

        // Prevent adding the same NbtTag instance more than once
        if (Fields.ContainsValue(value))
            return false;

        Fields[name] = value;
        return true;
    }
}
