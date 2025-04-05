using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public interface IPropertyType;

public interface IPropertyType<TPropertyValue> : IPropertyType where TPropertyValue : IPropertyValue
{
    public TPropertyValue Read(ref MinecraftBuffer buffer);
    public void Write(ref MinecraftBuffer buffer, TPropertyValue value);
}
