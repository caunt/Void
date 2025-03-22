using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Registries.Packets;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IPacketRegistryHolder? RegistryHolder { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}