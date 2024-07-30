using Void.Proxy.API.Network;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Registries.Packets;

public interface IPacketRegistryHolder
{
    public bool IsEmpty { get; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry? ClientboundRegistry { get; set; }
    public IPacketRegistry? ServerboundRegistry { get; set; }

    public IPacketRegistry? GetRegistry(Direction? flow, Operation? operation)
    {
        return (flow, operation) switch
        {
            (Direction.Clientbound, Operation.Write) => ServerboundRegistry,
            (Direction.Serverbound, Operation.Write) => ClientboundRegistry,
            (Direction.Clientbound, Operation.Read) => ClientboundRegistry,
            (Direction.Serverbound, Operation.Read) => ServerboundRegistry,
            _ => null
        };
    }

    public ProtocolVersion? GetProtocolVersion(Direction? flow)
    {
        return flow switch
        {
            Direction.Clientbound => ClientboundRegistry?.ProtocolVersion,
            Direction.Serverbound => ServerboundRegistry?.ProtocolVersion,
            _ => null
        };
    }

    public void Reset();
}