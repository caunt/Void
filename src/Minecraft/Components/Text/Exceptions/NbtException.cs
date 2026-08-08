using System;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Components.Text.Exceptions;

/// <summary>
/// Represents an error raised while working with NBT-backed text content.
/// </summary>
/// <param name="message">The exception message, or <c>null</c> to use the default <see cref="Exception"/> message.</param>
public class NbtException(string? message = null) : Exception(message)
{
    /// <summary>Creates an exception whose message is the SNBT representation of a tag.</summary>
    /// <param name="tag">The offending tag, or <see langword="null"/> for the default exception message.</param>
    public NbtException(NbtTag? tag = null) : this(tag?.ToString())
    {

    }
}
