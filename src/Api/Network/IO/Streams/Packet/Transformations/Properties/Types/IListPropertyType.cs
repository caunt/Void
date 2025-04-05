using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public interface IListPropertyType<TPropertyValue> : IPropertyType where TPropertyValue : IPropertyValue
{
    public List<TPropertyValue> Read(ref MinecraftBuffer buffer);
    public void Write(ref MinecraftBuffer buffer, List<TPropertyValue> value);
}
