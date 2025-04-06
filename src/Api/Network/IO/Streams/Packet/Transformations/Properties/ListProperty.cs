using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

public record ListProperty<TPacketProperty>(List<TPacketProperty> Values) : IPacketProperty<ListProperty<TPacketProperty>> where TPacketProperty : IPacketProperty<TPacketProperty>
{
    public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
    {
        var size = buffer.ReadVarInt();
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
