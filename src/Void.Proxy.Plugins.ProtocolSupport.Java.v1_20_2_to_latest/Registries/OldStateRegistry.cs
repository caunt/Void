using Void.Proxy.API.Network;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public class OldStateRegistry
{
    public OldPacketRegistry Clientbound { get; } = new(Direction.Clientbound);
    public OldPacketRegistry Serverbound { get; } = new(Direction.Serverbound);

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