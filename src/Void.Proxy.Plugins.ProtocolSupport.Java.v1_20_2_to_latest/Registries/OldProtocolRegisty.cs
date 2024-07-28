using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public class ProtocolRegistry(
    Direction direction,
    ProtocolVersion version)
{
    public ProtocolVersion Version { get; } = version;
    public Dictionary<int, PacketFactory> PacketIdToFactory { get; } = [];
    public Dictionary<Type, int> PacketTypeToId { get; } = [];

    public IMinecraftPacket? CreatePacket(int id)
    {
        return PacketIdToFactory.TryGetValue(id, out var factory) ? factory.Invoke() : null;
    }

    public int GetPacketId(IMinecraftPacket packet)
    {
        if (PacketTypeToId.TryGetValue(packet.GetType(), out var id))
            return id;

        throw new ArgumentException($"Unable to find id for {direction} packet of type {packet.GetType().Name} in protocol {Version}");
    }

    public bool ContainsPacket(IMinecraftPacket packet)
    {
        return PacketTypeToId.ContainsKey(packet.GetType());
    }
}