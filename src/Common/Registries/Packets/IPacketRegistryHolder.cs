using Void.Proxy.API.Network;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;
using Void.Proxy.Common.Network.Protocol;

namespace Void.Proxy.Common.Registries.Packets;

public interface IPacketRegistryHolder
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry ClientboundRegistry { get; set; }
    public IPacketRegistry ServerboundRegistry { get; set; }

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

    public void ReplacePackets(Direction direction, IReadOnlyDictionary<PacketMapping[], Type> mappings);
    public void AddPackets(Direction direction, IReadOnlyDictionary<PacketMapping[], Type> mappings);
    public void Reset();
}