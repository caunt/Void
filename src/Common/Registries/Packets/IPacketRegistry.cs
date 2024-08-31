using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Common.Registries.Packets;

public interface IPacketRegistry
{
    public ProtocolVersion ProtocolVersion { get; }

    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out PacketDecoder<IMinecraftPacket> packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public void RegisterPackets(IReadOnlyDictionary<PacketMapping[], Type> mappings);
}