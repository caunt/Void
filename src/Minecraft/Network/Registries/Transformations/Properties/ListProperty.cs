using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents a variable-length-prefixed list of packet properties.
/// </summary>
/// <typeparam name="TPacketProperty">The self-decoding property type stored in the list.</typeparam>
/// <param name="Values">The mutable element list, retained without copying.</param>
public record ListProperty<TPacketProperty>(List<TPacketProperty> Values) : IPacketProperty<ListProperty<TPacketProperty>> where TPacketProperty : IPacketProperty<TPacketProperty>
{
    /// <summary>
    /// Reads a list property from <paramref name="buffer"/> using the encoded element count prefix.
    /// </summary>
    /// <remarks>
    /// This method first reads the list length as a VarInt and then delegates to <see cref="Read(ref MinecraftBuffer, int)"/>
    /// to materialize each element by calling <c>TPacketProperty.Read(ref buffer)</c> repeatedly.
    /// </remarks>
    public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
    {
        return Read(ref buffer, buffer.ReadVarInt());
    }

    /// <summary>
    /// Reads an explicit number of elements without consuming a length prefix.
    /// </summary>
    /// <param name="buffer">The source buffer.</param>
    /// <param name="size">The number of elements to decode.</param>
    /// <returns>A property containing a new list of decoded elements.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size" /> is negative.</exception>
    public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer, int size)
    {
        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size));

        if (size == 0)
            return new([]);

        var list = new List<TPacketProperty>(size);

        for (var i = 0; i < size; i++)
        {
            var value = TPacketProperty.Read(ref buffer);
            list.Add(value);
        }

        return new(list);
    }

    /// <summary>
    /// Writes the current element count as a variable-length integer followed by each element.
    /// </summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Values.Count);

        foreach (var item in Values)
            item.Write(ref buffer);
    }
}
