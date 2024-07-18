using Void.Proxy.Network.Protocol.Packets;

namespace Void.Proxy.Network.Protocol.Registry;

public class ProtocolRegistry(
    Direction direction,
    ProtocolVersion version)
{
    public ProtocolVersion Version { get; } = version;
    public Dictionary<int, Func<IMinecraftPacket>> PacketIdToFactory { get; } = [];
    public Dictionary<Type, int> PacketTypeToId { get; } = [];

    public IMinecraftPacket? CreatePacket(int id)
    {
        if (PacketIdToFactory.TryGetValue(id, out var factory))
            return factory.Invoke();

        return null;
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