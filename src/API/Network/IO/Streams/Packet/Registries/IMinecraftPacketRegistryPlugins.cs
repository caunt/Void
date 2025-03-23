using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Network.IO.Streams.Packet.Registries;

public interface IMinecraftPacketRegistryPlugins
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketRegistry> All { get; }

    public IMinecraftPacketRegistry Get(IPlugin plugin);
    public void Remove(IPlugin plugin);
    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message);
    public bool Contains(Type type);
    public void ReplacePackets(IPlugin plugin, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void AddPackets(IPlugin plugin, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void Clear();
    public void Reset();
}