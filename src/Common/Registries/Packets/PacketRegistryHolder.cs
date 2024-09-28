using Void.Proxy.API.Network;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;
using Void.Proxy.Common.Network.Protocol;

namespace Void.Proxy.Common.Registries.Packets;

public class PacketRegistryHolder : IPacketRegistryHolder
{
    public bool IsEmpty => this is { ClientboundRegistry.IsEmpty: true, ServerboundRegistry.IsEmpty: true, ManagedBy: null };
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry ClientboundRegistry { get; set; } = new PacketRegistry();
    public IPacketRegistry ServerboundRegistry { get; set; } = new PacketRegistry();

    public void ReplacePackets(Direction direction, IReadOnlyDictionary<PacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        var registry = direction switch
        {
            Direction.Clientbound => ClientboundRegistry,
            Direction.Serverbound => ServerboundRegistry,
            _ => throw new ArgumentException(nameof(direction))
        };

        registry.ReplacePackets(mappings, ProtocolVersion);
    }

    public void AddPackets(Direction direction, IReadOnlyDictionary<PacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        var registry = direction switch
        {
            Direction.Clientbound => ClientboundRegistry,
            Direction.Serverbound => ServerboundRegistry,
            _ => throw new ArgumentException(nameof(direction))
        };

        registry.AddPackets(mappings, ProtocolVersion);
    }

    public void Reset()
    {
        ProtocolVersion = null;
        ManagedBy = null;

        ClientboundRegistry.Clear();
        ServerboundRegistry.Clear();
    }
}