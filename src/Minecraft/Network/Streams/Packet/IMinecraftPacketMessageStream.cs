using System.Threading;
using System.Threading.Tasks;
using Void.Common.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet.Registries;
using Void.Minecraft.Network.Streams.Packet.Transformations;

namespace Void.Minecraft.Network.Streams.Packet;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IMinecraftPacketSystemRegistry? SystemRegistryHolder { get; set; }
    public IMinecraftPacketPluginsRegistry? PluginsRegistryHolder { get; set; }
    public IMinecraftPacketPluginsTransformations? TransformationsHolder { get; set; }

    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);
    public void WritePacket(IMinecraftPacket packet, Side origin);
    public ValueTask WritePacketAsync(IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default);
}
