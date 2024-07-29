using Void.Proxy.API.Plugins;
using Void.Proxy.API.Registries.Packets;

namespace Void.Proxy.Registries.Packets;

public class PacketRegistryHolder : IPacketRegistryHolder
{
    public bool IsEmpty => this is { ClientRegistry: null, ServerRegistry: null, ManagedBy: null };
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry? ClientRegistry { get; set; }
    public IPacketRegistry? ServerRegistry { get; set; }

    public void Reset()
    {
        ManagedBy = null;
        ClientRegistry = null;
        ServerRegistry = null;
    }
}