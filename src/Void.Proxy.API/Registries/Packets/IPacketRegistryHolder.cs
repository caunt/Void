using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Registries.Packets;

public interface IPacketRegistryHolder
{
    public bool IsEmpty { get; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry? ClientboundRegistry { get; set; }
    public IPacketRegistry? ServerboundRegistry { get; set; }
}