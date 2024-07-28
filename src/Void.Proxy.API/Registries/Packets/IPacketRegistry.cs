using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API.Registries.Packets;

public interface IPacketRegistry
{
    public ProtocolVersion ProtocolVersion { get; }

    public bool TryCreatePacket(int id, [MaybeNullWhen(false)] out IMinecraftPacket packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public void RegisterPackets(IReadOnlyDictionary<PacketMapping[], PacketFactory> mappings);
}