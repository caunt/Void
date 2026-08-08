using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Void.Minecraft.Nbt.SharpNBT.Tags;

namespace Void.Minecraft.Nbt.Tags;

/// <summary>
/// Represents an NBT compound tag backed by a mutable dictionary of named child tags.
/// </summary>
/// <param name="Fields">
/// The dictionary used as the compound's backing storage. The dictionary instance is retained directly; changes made through it are reflected by this compound, and existing tag values are not renamed automatically.
/// </param>
public record NbtCompound(Dictionary<string, NbtTag> Fields) : NbtTag
{
    /// <summary>Creates an empty compound backed by a new dictionary.</summary>
    public NbtCompound() : this([])
    {

    }

    /// <summary>Gets or replaces a named child tag.</summary>
    /// <param name="name">The compound key.</param>
    /// <returns>The stored tag, or <see langword="null"/> when the key is absent.</returns>
    /// <exception cref="ArgumentNullException">The assigned value is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The assigned tag instance already occurs elsewhere in this compound.</exception>
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

    /// <summary>Converts a SharpNBT compound and its children.</summary>
    /// <param name="tag">The source compound.</param>
    /// <returns>The converted compound.</returns>
    public static implicit operator NbtCompound(CompoundTag tag) => new(tag.Values.ToDictionary(pair => pair.Name ?? string.Empty, pair => (NbtTag)pair)) { Name = tag.Name };
    /// <summary>Converts this compound and its children to SharpNBT.</summary>
    /// <param name="tag">The source compound.</param>
    /// <returns>The converted compound.</returns>
    public static implicit operator CompoundTag(NbtCompound tag) => new(tag.Name, tag.Fields.Select(pair => (Tag)pair.Value));

    /// <summary>Renames an existing field and updates the child tag's <see cref="NbtTag.Name"/>.</summary>
    /// <param name="name">The current key.</param>
    /// <param name="newName">The replacement key.</param>
    /// <exception cref="KeyNotFoundException">The source key is absent or the tag instance cannot be inserted at the new key.</exception>
    public void RenameKey(string name, string newName)
    {
        if (TryRenameKey(name, newName))
            return;

        throw new KeyNotFoundException($"Failed to rename key '{name}' to '{newName}'.");
    }

    /// <summary>Attempts to rename a field and update the child tag's name.</summary>
    /// <param name="name">The current key.</param>
    /// <param name="newName">The replacement key.</param>
    /// <returns><see langword="true"/> when the source existed and was reinserted; otherwise <see langword="false"/>.</returns>
    public bool TryRenameKey(string name, string newName) => Fields.TryGetValue(name, out var value) && Fields.Remove(name) && TrySetValue(newName, value);
    /// <summary>Determines whether the compound contains a key.</summary>
    /// <param name="name">The key to locate.</param>
    /// <returns><see langword="true"/> when the key exists.</returns>
    public bool ContainsKey(string name) => Fields.ContainsKey(name);
    /// <summary>Attempts to retrieve a child tag by key.</summary>
    /// <param name="name">The key to locate.</param>
    /// <param name="value">The child tag when found.</param>
    /// <returns><see langword="true"/> when the key exists.</returns>
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out NbtTag value) => Fields.TryGetValue(name, out value);
    /// <summary>Serializes this compound as SNBT.</summary>
    /// <returns>The SNBT representation.</returns>
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
