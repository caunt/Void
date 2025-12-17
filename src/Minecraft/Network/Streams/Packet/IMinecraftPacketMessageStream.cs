using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries;

namespace Void.Minecraft.Network.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IRegistryHolder Registries { get; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}
