using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.IO.Streams.Packet.Registries;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Registries;

public class MinecraftPacketRegistryPlugins : IMinecraftPacketRegistryPlugins
{
    private Dictionary<IPlugin, IMinecraftPacketRegistry> _map = [];

    public bool IsEmpty => All.All(registry => registry.IsEmpty);
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketRegistry> All => _map.Values;

    public IMinecraftPacketRegistry Get(IPlugin plugin)
    {
        if (!_map.TryGetValue(plugin, out var registry))
            _map[plugin] = registry = new MinecraftPacketRegistry();

        return registry;
    }

    public void Remove(IPlugin plugin)
    {
        _map.Remove(plugin);
    }

    public bool Contains<T>() where T : IMinecraftPacket
    {
        return All.Any(registry => registry.Contains<T>());
    }

    public bool Contains(IMinecraftMessage message)
    {
        return All.Any(registry => registry.Contains(message));
    }

    public bool Contains(Type type)
    {
        return All.Any(registry => registry.Contains(type));
    }

    public void ReplacePackets(IPlugin plugin, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        _map[plugin].ReplacePackets(mappings, ProtocolVersion);
    }

    public void AddPackets(IPlugin plugin, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        _map[plugin].AddPackets(mappings, ProtocolVersion);
    }

    public void Clear()
    {
        _map = [];
    }

    public void Reset()
    {
        Clear();
        ProtocolVersion = null;
        ManagedBy = null;
    }
}