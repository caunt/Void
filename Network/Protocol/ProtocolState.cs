using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;

namespace MinecraftProxy.Network.Protocol;

public interface IPlayableState : IProtocolState
{
    public Task<bool> HandleAsync(DisconnectPacket packet);
}

public interface IProtocolState { }

public abstract class ProtocolState : IProtocolState
{
    protected abstract Dictionary<int, Type> clientboundPackets { get; }
    protected abstract Dictionary<int, Type> serverboundPackets { get; }

    public IMinecraftPacket<T>? Decode<T>(PacketDirection direction, int packetId, MinecraftBuffer buffer) where T : ProtocolState
    {
        var packets = direction switch
        {
            PacketDirection.Clientbound => clientboundPackets,
            PacketDirection.Serverbound => serverboundPackets,
            _ => throw new ArgumentException(nameof(direction))
        };

        if (packetId == -1 || !packets.TryGetValue(packetId, out Type? packetType))
            return null;

        var packet = Activator.CreateInstance(packetType) as IMinecraftPacket<T> ?? throw new Exception($"Cannot create instance of {packetType} packet");
        packet.Decode(buffer);

        if (buffer.HasData)
            throw new IOException($"{direction} packet {packetType.Name} has extra data ({buffer.Position} < {buffer.Length})");

        return packet;
    }

    public int? FindPacketId(PacketDirection direction, IMinecraftPacket packet)
    {
        var packets = direction switch
        {
            PacketDirection.Clientbound => clientboundPackets,
            PacketDirection.Serverbound => serverboundPackets,
            _ => throw new ArgumentException(nameof(direction))
        };

        var packetType = packet.GetType();

        return packets.Where(x => x.Value == packetType).Select(x => x.Key).FirstOrDefault();
    }
}