using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.PacketId;

/// <summary>
/// Partitions read and write packet-ID registries by contributing plugin.
/// </summary>
public interface IMinecraftPacketIdPluginsRegistry
{
    /// <summary>
    /// Gets whether all plugin-owned read and write packet ID registries are empty.
    /// </summary>
    /// <value>
    /// <see langword="true"/> when every registry in <see cref="Read"/> and <see cref="Write"/> contains no packet ID
    /// mappings; otherwise, <see langword="false"/>. An instance with no plugin registries is considered empty.
    /// </value>
    public bool IsEmpty { get; }
    /// <summary>Gets or sets the protocol version for which plugin mappings are configured.</summary>
    public ProtocolVersion? ProtocolVersion { get; set; }
    /// <summary>Gets or sets the plugin responsible for this aggregate registry's lifecycle.</summary>
    public IPlugin? ManagedBy { get; set; }
    /// <summary>Gets the current plugin-owned read registries.</summary>
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> Read { get; }
    /// <summary>Gets the current plugin-owned write registries.</summary>
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> Write { get; }

    /// <summary>Gets or creates the registry for one plugin and one operation.</summary>
    /// <param name="operation">Exactly <see cref="Operation.Read" /> or <see cref="Operation.Write" />.</param>
    /// <param name="plugin">The plugin that owns the returned registry.</param>
    /// <returns>The existing or newly created registry.</returns>
    /// <exception cref="InvalidOperationException"><see cref="ProtocolVersion" /> has not been set.</exception>
    /// <exception cref="ArgumentException"><paramref name="operation" /> is <see cref="Operation.Any" /> or an unsupported value.</exception>
    public IMinecraftPacketIdRegistry Get(Operation operation, IPlugin plugin);
    /// <summary>Attempts to find the first plugin with a compatible mapping for a packet type.</summary>
    /// <typeparam name="T">The packet type to locate.</typeparam>
    /// <param name="plugin">When successful, the owning plugin; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when a mapping is found; otherwise, <see langword="false" />.</returns>
    public bool TryGetPlugin<T>([MaybeNullWhen(false)] out IPlugin plugin) where T : IMinecraftPacket;
    /// <summary>Attempts to find the first plugin with a compatible mapping for a message's runtime type.</summary>
    /// <param name="message">The message to locate.</param>
    /// <param name="plugin">When successful, the owning plugin; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when a mapping is found; otherwise, <see langword="false" />.</returns>
    public bool TryGetPlugin(INetworkMessage message, [MaybeNullWhen(false)] out IPlugin plugin);
    /// <summary>Attempts to find the first plugin with a compatible mapping for a runtime type.</summary>
    /// <param name="type">The packet type to locate.</param>
    /// <param name="plugin">When successful, the owning plugin; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when a mapping is found; otherwise, <see langword="false" />.</returns>
    public bool TryGetPlugin(Type type, [MaybeNullWhen(false)] out IPlugin plugin);
    /// <summary>Removes both read and write registries owned by a plugin.</summary>
    /// <param name="plugin">The plugin to remove.</param>
    public void Remove(IPlugin plugin);
    /// <summary>Determines whether any plugin registry contains a compatible packet type.</summary>
    /// <typeparam name="T">The packet type to query.</typeparam>
    /// <returns><see langword="true" /> when any read or write registry contains it; otherwise, <see langword="false" />.</returns>
    public bool Contains<T>() where T : IMinecraftPacket;
    /// <summary>Determines whether any plugin registry contains a compatible mapping for a message.</summary>
    /// <param name="message">The message to query.</param>
    /// <returns><see langword="true" /> when any read or write registry contains it; otherwise, <see langword="false" />.</returns>
    public bool Contains(INetworkMessage message);
    /// <summary>Determines whether any plugin registry contains a compatible runtime type.</summary>
    /// <param name="type">The type to query.</param>
    /// <returns><see langword="true" /> when any read or write registry contains it; otherwise, <see langword="false" />.</returns>
    public bool Contains(Type type);
    /// <summary>Removes all per-plugin read and write registries while preserving ownership and protocol metadata.</summary>
    public void Clear();
    /// <summary>
    /// Clears packet ID registrations for the specified direction from the selected operation buckets.
    /// </summary>
    /// <remarks>
    /// When <paramref name="operation"/> includes <see cref="Operation.Read"/>, each read registry is cleared for
    /// <paramref name="direction"/>. When it includes <see cref="Operation.Write"/>, each write registry is cleared.
    /// If neither flag is present, this method has no effect.
    /// </remarks>
    /// <param name="direction">The packet direction whose registrations should be removed.</param>
    /// <param name="operation">The read and/or write registry buckets to clear.</param>
    public void Clear(Direction direction, Operation operation);
    /// <summary>Clears all plugin registries and sets <see cref="ProtocolVersion" /> and <see cref="ManagedBy" /> to <see langword="null" />.</summary>
    public void Reset();
}
