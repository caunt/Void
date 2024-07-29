using Void.Proxy.API.Network;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Registries.Packets;

public interface IPacketRegistryHolder
{
    public bool IsEmpty { get; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry? ClientRegistry { get; set; }
    public IPacketRegistry? ServerRegistry { get; set; }

    public IPacketRegistry? GetRegistry(Direction? flow, Operation? operation)
    {
        return (flow, operation) switch
        {
            (Direction.Clientbound, Operation.Write) => ServerRegistry,
            (Direction.Serverbound, Operation.Write) => ClientRegistry,
            (Direction.Clientbound, Operation.Read) => ClientRegistry,
            (Direction.Serverbound, Operation.Read) => ServerRegistry,
            _ => null
        };
    }
    public void Reset();
}