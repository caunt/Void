using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Network.Protocol.Registry;

public class StateRegistry
{
    public PacketRegistry Clientbound { get; } = new(Direction.Clientbound);
    public PacketRegistry Serverbound { get; } = new(Direction.Serverbound);

    public ProtocolRegistry GetProtocolRegistry(Direction direction, ProtocolVersion version)
    {
        return direction switch
        {
            Direction.Serverbound => Serverbound.GetProtocolRegistry(version),
            Direction.Clientbound => Clientbound.GetProtocolRegistry(version),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
}