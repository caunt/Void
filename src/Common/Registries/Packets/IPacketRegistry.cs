using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;
using Void.Proxy.Common.Network.Protocol;

namespace Void.Proxy.Common.Registries.Packets;

public interface IPacketRegistry
{
    public bool IsEmpty { get; }

    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out PacketDecoder<IMinecraftPacket> packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public IPacketRegistry ReplacePackets(IReadOnlyDictionary<PacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IPacketRegistry AddPackets(IReadOnlyDictionary<PacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}