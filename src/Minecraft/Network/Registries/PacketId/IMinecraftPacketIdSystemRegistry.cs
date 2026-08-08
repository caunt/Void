using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.PacketId;

/// <summary>
/// Defines an interface for managing and accessing Minecraft packet ID registries for different protocol versions and
/// operations.
/// </summary>
/// <remarks>This interface has split <see cref="Read"/> and <see cref="Write"/> registries unlike <see cref="IMinecraftPacketIdPluginsRegistry"/>.</remarks>
public interface IMinecraftPacketIdSystemRegistry
{
    /// <summary>Gets whether the built-in registry has no read or write mappings and no managing plugin.</summary>
    public bool IsEmpty { get; }
    /// <summary>Gets or sets the protocol version used when selecting mappings.</summary>
    public ProtocolVersion? ProtocolVersion { get; set; }
    /// <summary>Gets or sets the plugin responsible for system registry lifecycle.</summary>
    public IPlugin? ManagedBy { get; set; }
    /// <summary>Gets or sets the registry used to decode inbound packets.</summary>
    public IMinecraftPacketIdRegistry Read { get; set; }
    /// <summary>Gets or sets the registry used to encode outbound packets.</summary>
    public IMinecraftPacketIdRegistry Write { get; set; }

    /// <summary>Determines whether either operation registry contains a compatible message type.</summary>
    /// <typeparam name="T">The message type to query.</typeparam>
    /// <returns><see langword="true" /> when a compatible read or write mapping exists; otherwise, <see langword="false" />.</returns>
    public bool Contains<T>() where T : IMinecraftMessage;
    /// <summary>Determines whether either operation registry contains a compatible runtime type.</summary>
    /// <param name="type">The type to query.</param>
    /// <returns><see langword="true" /> when a compatible read or write mapping exists; otherwise, <see langword="false" />.</returns>
    public bool Contains(Type type);
    /// <summary>Determines whether either operation registry contains a compatible mapping for a message.</summary>
    /// <param name="packet">The message to query.</param>
    /// <returns><see langword="true" /> when a compatible read or write mapping exists; otherwise, <see langword="false" />.</returns>
    public bool Contains(IMinecraftMessage packet);
    /// <summary>Clears and replaces mappings in selected operation registries for <see cref="ProtocolVersion" />.</summary>
    /// <param name="operation">The read, write, or both operation registries to replace.</param>
    /// <param name="mappings">Packet types keyed by ordered mapping arrays.</param>
    /// <exception cref="InvalidOperationException"><see cref="ProtocolVersion" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException"><paramref name="operation" /> is unsupported or selected mappings are invalid or conflicting.</exception>
    public void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings);
    /// <summary>Adds mappings to selected operation registries for <see cref="ProtocolVersion" />.</summary>
    /// <param name="operation">The read, write, or both operation registries to update.</param>
    /// <param name="mappings">Packet types keyed by ordered mapping arrays.</param>
    /// <exception cref="InvalidOperationException"><see cref="ProtocolVersion" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException"><paramref name="operation" /> is unsupported or selected mappings are invalid or conflicting.</exception>
    public void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings);
    /// <summary>Clears both operation registries and sets protocol and manager metadata to <see langword="null" />.</summary>
    public void Reset();
}
