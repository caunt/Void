﻿using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.Registries.Transformations;

public class MinecraftPacketTransformationsPluginsRegistry : IMinecraftPacketTransformationsPluginsRegistry
{
    private Dictionary<IPlugin, IMinecraftPacketTransformationsRegistry> _map = [];

    public bool IsEmpty => All.All(registry => registry.IsEmpty);
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketTransformationsRegistry> All => _map.Values;

    public IMinecraftPacketTransformationsRegistry Get(IPlugin plugin)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        if (!_map.TryGetValue(plugin, out var registry))
            _map[plugin] = registry = new MinecraftPacketTransformationsRegistry();

        return registry;
    }

    public void Remove(IPlugin plugin)
    {
        _map.Remove(plugin);
    }

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket
    {
        return All.Any(registry => registry.Contains<T>(type));
    }

    public bool Contains(IMinecraftMessage message, TransformationType type)
    {
        return All.Any(registry => registry.Contains(message, type));
    }

    public bool Contains(Type packetType, TransformationType transformationType)
    {
        return All.Any(registry => registry.Contains(packetType, transformationType));
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
