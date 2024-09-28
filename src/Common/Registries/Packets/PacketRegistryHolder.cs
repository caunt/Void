using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Common.Registries.Packets;

public class PacketRegistryHolder : IPacketRegistryHolder
{
    public bool IsEmpty => this is { ClientboundRegistry.IsEmpty: true, ServerboundRegistry.IsEmpty: true, ManagedBy: null };
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry ClientboundRegistry { get; set; } = new PacketRegistry();
    public IPacketRegistry ServerboundRegistry { get; set; } = new PacketRegistry();

    public void Reset()
    {
        ProtocolVersion = null;
        ManagedBy = null;

        ClientboundRegistry.Clear();
        ServerboundRegistry.Clear();
    }
}