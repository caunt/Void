using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations.Properties;

public record PacketProperty(IPropertyType Type, IPropertyValue Value) : IPacketProperty;
