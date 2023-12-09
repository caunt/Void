using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Registry;

namespace MinecraftProxy.Network.Protocol.States;

public interface IProtocolState { }

public abstract class ProtocolState : IProtocolState
{
    protected abstract StateRegistry Registry { get; }

    public IMinecraftPacket<T>? Decode<T>(int packetId, Direction direction, ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where T : ProtocolState
    {
        var protocolRegistry = Registry.GetProtocolRegistry(direction, protocolVersion);
        var packet = protocolRegistry.CreatePacket(packetId);

        if (packet is null)
            return null; // packet not registered, proceed

        packet.Decode(ref buffer, protocolVersion);

        if (buffer.HasData)
            throw new IOException($"{direction} packet {packet} has extra data ({buffer.Position} < {buffer.Length})");

        return packet as IMinecraftPacket<T> ?? throw new Exception($"Cannot cast instance of {packetId} packet to {typeof(IMinecraftPacket<T>)}");
    }

    public int? FindPacketId(Direction direction, IMinecraftPacket packet, ProtocolVersion protocolVersion)
    {
        var protocolRegistry = Registry.GetProtocolRegistry(direction, protocolVersion);

        if (protocolRegistry is null)
            return null;

        return protocolRegistry.GetPacketId(packet);
    }
}