using System.Threading;
using System.Threading.Tasks;
using Void.Common.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries;

namespace Void.Minecraft.Network.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IRegistryHolder Registries { get; }

    public IMinecraftPacket ReadPacket(Side origin);
    public ValueTask<IMinecraftPacket> ReadPacketAsync(Side origin, CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet, Side origin);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default);
}
