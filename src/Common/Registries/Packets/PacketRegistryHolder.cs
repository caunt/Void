using Void.Proxy.API.Plugins;

namespace Void.Proxy.Common.Registries.Packets;

public class PacketRegistryHolder : IPacketRegistryHolder
{
    public bool IsEmpty => this is { ClientboundRegistry: null, ServerboundRegistry: null, ManagedBy: null };
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry? ClientboundRegistry { get; set; }
    public IPacketRegistry? ServerboundRegistry { get; set; }

    public void Reset()
    {
        ManagedBy = null;
        ClientboundRegistry = null;
        ServerboundRegistry = null;
    }
}