using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

public interface IMinecraftPacketRegistryPlugins
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketRegistrySystem> All { get; }

    public IMinecraftPacketRegistry Get(IPlugin plugin, Operation operation);
    public void Remove(IPlugin plugin);
    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message);
    public bool Contains(Type type);
    public void ReplacePackets(IPlugin plugin, Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void AddPackets(IPlugin plugin, Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void Clear();
    public void Reset();
}