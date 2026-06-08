using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

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

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Values.Count);

        foreach (var item in Values)
            item.Write(ref buffer);
    }
}
