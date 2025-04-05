using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Registries;

public class MinecraftPacketPluginsRegistry : IMinecraftPacketPluginsRegistry
{
    private Dictionary<IPlugin, IMinecraftPacketRegistry> _map = [];

    public bool IsEmpty => All.All(registry => registry.IsEmpty);
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketRegistry> All => _map.Values;

    public IMinecraftPacketRegistry Get(IPlugin plugin)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        if (!_map.TryGetValue(plugin, out var registry))
            _map[plugin] = registry = new MinecraftPacketRegistry();

        return registry;
    }

    public IPlugin GetPlugin<T>() where T : IMinecraftPacket
    {
        return GetPlugin(typeof(T));
    }

    public IPlugin GetPlugin(IMinecraftMessage message)
    {
        return GetPlugin(message.GetType());
    }

    public IPlugin GetPlugin(Type type)
    {
        foreach (var (plugin, registry) in _map)
        {
            if (registry.Contains(type))
                return plugin;

        }

        throw new InvalidOperationException("No plugin found for the given message");
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