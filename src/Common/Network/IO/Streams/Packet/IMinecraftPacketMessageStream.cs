using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.Common.Network.IO.Messages;
using Void.Proxy.Common.Registries.Packets;

namespace Void.Proxy.Common.Network.IO.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IPacketRegistryHolder? RegistryHolder { get; set; }
    public Direction? Flow { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}