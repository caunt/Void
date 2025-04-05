using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record ListType<TPropertyValue>(IPropertyType<TPropertyValue> Type) : IListPropertyType<TPropertyValue> where TPropertyValue : IPropertyValue
{
    public List<TPropertyValue> Read(ref MinecraftBuffer buffer)
    {
        var size = buffer.ReadVarInt();
        if (size == 0)
            return [];
        
        var list = new List<TPropertyValue>(size);
        
        for (var i = 0; i < size; i++)
        {
            var value = Type.Read(ref buffer);
            list.Add(value);
        }

        return list;
    }

    public void Write(ref MinecraftBuffer buffer, List<TPropertyValue> value)
    {
        buffer.WriteVarInt(value.Count);
        foreach (var item in value)
            Type.Write(ref buffer, item);
    }
}
