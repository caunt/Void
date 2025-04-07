using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Api.Network.IO.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IMinecraftPacketSystemRegistry? SystemRegistryHolder { get; set; }
    public IMinecraftPacketPluginsRegistry? PluginsRegistryHolder { get; set; }
    public IMinecraftPacketPluginsTransformations? TransformationsHolder { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}
