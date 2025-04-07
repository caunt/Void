using System.Threading;
using System.Threading.Tasks;
using Void.Common.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;

namespace Void.Minecraft.Network.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IMinecraftPacketIdSystemRegistry? SystemRegistryHolder { get; set; }
    public IMinecraftPacketIdPluginsRegistry? PluginsRegistryHolder { get; set; }
    public IMinecraftPacketPluginsTransformations? TransformationsHolder { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet, Side origin);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default);
}
