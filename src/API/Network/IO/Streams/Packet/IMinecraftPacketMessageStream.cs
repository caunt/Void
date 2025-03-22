using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.API.Network.IO.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IMinecraftPacketRegistryHolder? RegistryHolder { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}