using System;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Components.Text.Exceptions;

/// <summary>
/// Represents an error raised while working with NBT-backed text content.
/// </summary>
/// <param name="message">The exception message, or <c>null</c> to use the default <see cref="Exception"/> message.</param>
public class NbtException(string? message = null) : Exception(message)
{
    public NbtException(NbtTag? tag = null) : this(tag?.ToString())
    {

    }
}
