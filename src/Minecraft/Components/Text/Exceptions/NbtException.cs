using System;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Components.Text.Exceptions;

public class NbtException(string? message = null) : Exception(message)
{
    public NbtException(NbtTag? tag = null) : this(tag?.ToString())
    {

    }
}
