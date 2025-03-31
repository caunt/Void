using Void.Minecraft.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Registries;

public class MinecraftPacketRegistryPlugins : IMinecraftPacketRegistryPlugins
{
    private Dictionary<IPlugin, IMinecraftPacketRegistrySystem> _map = [];

    public bool IsEmpty => All.All(registry => registry.IsEmpty);
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketRegistrySystem> All => _map.Values;

    public IMinecraftPacketRegistry Get(IPlugin plugin, Operation operation)
    {
        if (!_map.TryGetValue(plugin, out var registry))
            _map[plugin] = registry = new MinecraftPacketRegistrySystem { ProtocolVersion = ProtocolVersion, ManagedBy = ManagedBy };

        return operation switch
        {
            Operation.Read => registry.Read,
            Operation.Write => registry.Write,
            _ => throw new ArgumentException($"Invalid operation {operation}", nameof(operation)),
        };
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

    public void ReplacePackets(IPlugin plugin, Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        _map[plugin].ReplacePackets(operation, mappings);
    }

    public void AddPackets(IPlugin plugin, Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        _map[plugin].AddPackets(operation, mappings);
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