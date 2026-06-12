using System;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries;

public interface IRegistryHolder : IDisposable
{
    public ProtocolVersion ProtocolVersion { get; }

    public IMinecraftPacketIdSystemRegistry PacketIdSystem { get; }
    public IMinecraftPacketIdPluginsRegistry PacketIdPlugins { get; }
    public IMinecraftPacketTransformationsSystemRegistry PacketTransformationsSystem { get; }
    public IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; }

    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion);
    public void ClearPlugin(IPlugin plugin);
    public void ClearPlugins();
    public void ClearPlugins(Direction direction, Operation operation);
    public string PrintPackets();

    /// <summary>
    /// Resets the held registries when they are all managed by the specified plugin.
    /// </summary>
    /// <param name="managedBy">The plugin that must own every held registry before disposal occurs.</param>
    public void DisposeBy(IPlugin managedBy);
}
