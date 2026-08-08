using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.Transformations;

/// <summary>
/// Partitions packet transformation registries by contributing plugin.
/// </summary>
public interface IMinecraftPacketTransformationsPluginsRegistry
{
    /// <summary>Gets whether every plugin registry contains no transformation pipelines.</summary>
    public bool IsEmpty { get; }
    /// <summary>Gets or sets the protocol version for which transformations are configured.</summary>
    public ProtocolVersion? ProtocolVersion { get; set; }
    /// <summary>Gets or sets the plugin responsible for this aggregate registry's lifecycle.</summary>
    public IPlugin? ManagedBy { get; set; }
    /// <summary>Gets the current plugin-owned transformation registries.</summary>
    public IReadOnlyCollection<IMinecraftPacketTransformationsRegistry> All { get; }

    /// <summary>Gets or creates the transformation registry owned by a plugin.</summary>
    /// <param name="plugin">The plugin that owns the returned registry.</param>
    /// <returns>The existing or newly created registry.</returns>
    /// <exception cref="InvalidOperationException"><see cref="ProtocolVersion" /> has not been set.</exception>
    public IMinecraftPacketTransformationsRegistry Get(IPlugin plugin);
    /// <summary>Removes the registry owned by a plugin.</summary>
    /// <param name="plugin">The plugin to remove.</param>
    public void Remove(IPlugin plugin);
    /// <summary>Determines whether any plugin registry contains a transformation for a packet type.</summary>
    /// <typeparam name="T">The packet type to query.</typeparam>
    /// <param name="type">The transformation direction to query.</param>
    /// <returns><see langword="true" /> when a matching transformation exists; otherwise, <see langword="false" />.</returns>
    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    /// <summary>Determines whether any plugin registry contains a transformation for a message.</summary>
    /// <param name="message">The message to query.</param>
    /// <param name="type">The transformation direction to query.</param>
    /// <returns><see langword="true" /> when a matching transformation exists; otherwise, <see langword="false" />.</returns>
    public bool Contains(IMinecraftMessage message, TransformationType type);
    /// <summary>Determines whether any plugin registry contains a transformation for a runtime type.</summary>
    /// <param name="packetType">The packet type to query.</param>
    /// <param name="transformationType">The transformation direction to query.</param>
    /// <returns><see langword="true" /> when a matching transformation exists; otherwise, <see langword="false" />.</returns>
    public bool Contains(Type packetType, TransformationType transformationType);
    /// <summary>Removes all per-plugin transformation registries while preserving metadata.</summary>
    public void Clear();
    /// <summary>Clears transformations for a packet direction in every plugin registry.</summary>
    /// <remarks>The built-in implementation currently ignores <paramref name="operation" /> because transformations are not partitioned by operation.</remarks>
    /// <param name="direction">The packet direction to clear.</param>
    /// <param name="operation">Reserved operation selection; currently ignored by the built-in implementation.</param>
    public void Clear(Direction direction, Operation operation);
    /// <summary>Clears all plugin registries and sets protocol and manager metadata to <see langword="null" />.</summary>
    public void Reset();
}
