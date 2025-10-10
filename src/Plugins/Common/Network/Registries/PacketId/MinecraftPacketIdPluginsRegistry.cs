using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.Registries.PacketId;

public class MinecraftPacketIdPluginsRegistry : IMinecraftPacketIdPluginsRegistry
{
    private readonly Dictionary<IPlugin, IMinecraftPacketIdRegistry> _read = [];
    private readonly Dictionary<IPlugin, IMinecraftPacketIdRegistry> _write = [];

    private IEnumerable<IMinecraftPacketIdRegistry> All => Read.Concat(Write);

    public bool IsEmpty => All.All(registry => registry.IsEmpty);
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> Read => _read.Values;
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> Write => _write.Values;

    public IMinecraftPacketIdRegistry Get(Operation operation, IPlugin plugin)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException($"{nameof(ProtocolVersion)} is not set yet");

        var map = operation switch
        {
            Operation.Read => _read,
            Operation.Write => _write,
            Operation.Any => throw new ArgumentException($"Operation {operation} is not valid here, use {nameof(Get)} twice instead"),
            _ => throw new ArgumentException($"Invalid operation {operation}", nameof(operation))
        };

        if (!map.TryGetValue(plugin, out var registry))
        {
            lock (this)
                map[plugin] = registry = new MinecraftPacketIdRegistry();
        }

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

        foreach (var (candidate, registry) in _read.Concat(_write))
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
        lock (this)
        {
            _read.Remove(plugin);
            _write.Remove(plugin);
        }
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
        lock (this)
        {
            _read.Clear();
            _write.Clear();
        }
    }

    public void Clear(Direction direction)
    {
        foreach (var registry in All)
            registry.Clear(direction);
    }

    public void Reset()
    {
        Clear();
        ProtocolVersion = null;
        ManagedBy = null;
    }
}
