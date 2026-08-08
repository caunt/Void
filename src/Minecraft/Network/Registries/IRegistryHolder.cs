using System;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries;

/// <summary>
/// Groups the system and plugin packet-ID and transformation registries for one network channel.
/// </summary>
public interface IRegistryHolder : IDisposable
{
    /// <summary>
    /// Gets the protocol version configured for the registries.
    /// </summary>
    /// <value>The configured version; the built-in holder falls back to <see cref="ProtocolVersion.Oldest" /> before setup.</value>
    public ProtocolVersion ProtocolVersion { get; }

    /// <summary>Gets packet-ID mappings owned by the system protocol plugin.</summary>
    public IMinecraftPacketIdSystemRegistry PacketIdSystem { get; }
    /// <summary>Gets packet-ID mappings partitioned by contributing plugin.</summary>
    public IMinecraftPacketIdPluginsRegistry PacketIdPlugins { get; }
    /// <summary>Gets packet transformations owned by the system protocol plugin.</summary>
    public IMinecraftPacketTransformationsSystemRegistry PacketTransformationsSystem { get; }
    /// <summary>Gets packet transformations partitioned by contributing plugin.</summary>
    public IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; }

    /// <summary>
    /// Assigns a common owner and protocol version to all held registries.
    /// </summary>
    /// <param name="managedBy">The plugin responsible for registry lifecycle.</param>
    /// <param name="protocolVersion">The channel protocol version used to select mappings.</param>
    /// <exception cref="InvalidOperationException">Any held registry already has an owner.</exception>
    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion);

    /// <summary>Removes packet IDs and transformations contributed by one plugin.</summary>
    /// <param name="plugin">The plugin whose entries are removed.</param>
    public void ClearPlugin(IPlugin plugin);

    /// <summary>Clears every plugin-contributed packet ID and transformation while preserving system registries.</summary>
    public void ClearPlugins();

    /// <summary>Clears plugin-contributed registrations for selected directions and operations.</summary>
    /// <param name="direction">The packet direction to remove.</param>
    /// <param name="operation">The read and/or write operations to remove.</param>
    public void ClearPlugins(Direction direction, Operation operation);

    /// <summary>Formats the packet types currently present in each held registry.</summary>
    /// <returns>A multiline diagnostic description of system and plugin packet registries.</returns>
    public string PrintPackets();

    /// <summary>
    /// Resets the held registries when they are all managed by the specified plugin.
    /// </summary>
    /// <param name="managedBy">The plugin that must own every held registry before disposal occurs.</param>
    public void DisposeBy(IPlugin managedBy);
}
