using System;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.Transformations;

/// <summary>
/// Provides the system-owned packet transformation registry and its lifecycle metadata.
/// </summary>
public interface IMinecraftPacketTransformationsSystemRegistry
{
    /// <summary>Gets whether the underlying transformation registry is empty.</summary>
    public bool IsEmpty { get; }
    /// <summary>Gets or sets the protocol version for which transformations are configured.</summary>
    public ProtocolVersion? ProtocolVersion { get; set; }
    /// <summary>Gets or sets the plugin responsible for system registry lifecycle.</summary>
    public IPlugin? ManagedBy { get; set; }
    /// <summary>Gets the underlying upgrade and downgrade transformation registry.</summary>
    public IMinecraftPacketTransformationsRegistry All { get; }

    /// <summary>Determines whether a transformation exists for a packet type.</summary>
    /// <typeparam name="T">The packet type to query.</typeparam>
    /// <param name="type">The transformation direction to query.</param>
    /// <returns><see langword="true" /> when a matching transformation exists; otherwise, <see langword="false" />.</returns>
    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    /// <summary>
    /// Determines whether the system registry contains a transformation of the specified type for the runtime type of a message.
    /// </summary>
    /// <param name="message">The message whose runtime type is used to locate a registered transformation.</param>
    /// <param name="type">The direction of transformation to locate.</param>
    /// <returns><see langword="true" /> if a matching transformation is registered; otherwise, <see langword="false" />.</returns>
    public bool Contains(IMinecraftMessage message, TransformationType type);
    /// <summary>Determines whether a transformation exists for a runtime packet type.</summary>
    /// <param name="packetType">The type to query.</param>
    /// <param name="transformationType">The transformation direction to query.</param>
    /// <returns><see langword="true" /> when a matching transformation exists; otherwise, <see langword="false" />.</returns>
    public bool Contains(Type packetType, TransformationType transformationType);
    /// <summary>Removes all system upgrade and downgrade pipelines while preserving metadata.</summary>
    public void Clear();
    /// <summary>Clears all pipelines and sets protocol and manager metadata to <see langword="null" />.</summary>
    public void Reset();
}
