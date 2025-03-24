using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

namespace Void.Proxy.Api.Network.IO.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IMinecraftPacketRegistrySystem? SystemRegistryHolder { get; set; }
    public IMinecraftPacketRegistryPlugins? PluginsRegistryHolder { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}