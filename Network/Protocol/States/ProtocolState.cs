using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.Packets;

namespace MinecraftProxy.Network.Protocol.States;

public interface IProtocolState { }

public abstract class ProtocolState : IProtocolState
{
    protected abstract Dictionary<int, Type> clientboundPackets { get; }
    protected abstract Dictionary<int, Type> serverboundPackets { get; }

    public IMinecraftPacket<T>? Decode<T>(int packetId, PacketDirection direction, ref MinecraftBuffer buffer) where T : ProtocolState
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
        packet.Decode(ref buffer);

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