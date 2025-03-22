using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;

namespace Void.Proxy.Plugins.Common.Registries.Packets;

public interface IPacketRegistryHolder
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IPacketRegistry Read { get; set; }
    public IPacketRegistry Write { get; set; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public void ReplacePackets(Operation operation, IReadOnlyDictionary<PacketMapping[], Type> mappings);
    public void AddPackets(Operation operation, IReadOnlyDictionary<PacketMapping[], Type> mappings);
    public void Reset();
}