using MinecraftProxy.Network.Protocol.Packets;

namespace MinecraftProxy.Network.Protocol.Registry;

public class StateRegistry
{
    public PacketRegistry Clientbound { get; } = new(PacketDirection.Clientbound);
    public PacketRegistry Serverbound { get; } = new(PacketDirection.Serverbound);

    public ProtocolRegistry GetProtocolRegistry(PacketDirection direction, ProtocolVersion version) => direction switch
    {
        PacketDirection.Serverbound => Serverbound.GetProtocolRegistry(version),
        PacketDirection.Clientbound => Clientbound.GetProtocolRegistry(version),
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}
