using System.Diagnostics.CodeAnalysis;
using Void.Common.Network.Messages;
using Void.Common.Plugins;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet.Registries;

namespace Void.Proxy.Plugins.Common.Network.Streams.Packet.Registries;

public class MinecraftPacketPluginsRegistry : IMinecraftPacketPluginsRegistry
{
    private Dictionary<IPlugin, IMinecraftPacketIdRegistry> _map = [];

    public bool IsEmpty => All.All(registry => registry.IsEmpty);
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> All => _map.Values;

    public IMinecraftPacketIdRegistry Get(IPlugin plugin)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        if (!_map.TryGetValue(plugin, out var registry))
            _map[plugin] = registry = new MinecraftPacketIdRegistry();

        return registry;
    }

    public bool TryGetPlugin<T>([MaybeNullWhen(false)] out IPlugin plugin) where T : IMinecraftPacket
    {
        return TryGetPlugin(typeof(T), out plugin);
    }

    public bool TryGetPlugin(INetworkMessage message, [MaybeNullWhen(false)] out IPlugin plugin)
    {
        return TryGetPlugin(message.GetType(), out plugin);
    }

    public bool TryGetPlugin(Type type, [MaybeNullWhen(false)] out IPlugin plugin)
    {
        plugin = null;

        foreach (var (candidate, registry) in _map)
        {
            if (registry.Contains(type))
            {
                plugin = candidate;
                return true;
            }
        }

        return false;
    }

    public void Remove(IPlugin plugin)
    {
        _map.Remove(plugin);
    }

    public bool Contains<T>() where T : IMinecraftPacket
    {
        return All.Any(registry => registry.Contains<T>());
    }

    public bool Contains(INetworkMessage message)
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
