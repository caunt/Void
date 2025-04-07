using System.Collections.Generic;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Streams.Packet.Transformations.Properties;

public record ListProperty<TPacketProperty>(List<TPacketProperty> Values) : IPacketProperty<ListProperty<TPacketProperty>> where TPacketProperty : IPacketProperty<TPacketProperty>
{
    public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
    {
        return Read(ref buffer, buffer.ReadVarInt());
    }

    public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer, int size)
    {
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
