using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

public interface IPacketProperty
{
    public IPropertyType Type { get; }
    public IPropertyValue Value { get; }
}
