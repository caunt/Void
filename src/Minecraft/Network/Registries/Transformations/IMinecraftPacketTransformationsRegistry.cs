using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Network;

namespace Void.Minecraft.Network.Registries.Transformations;

/// <summary>
/// Stores ordered upgrade and downgrade transformation pipelines by packet type.
/// </summary>
public interface IMinecraftPacketTransformationsRegistry
{
    /// <summary>
    /// Gets the packet <see cref="Type"/> values that currently have transformation entries in this registry.
    /// </summary>
    /// <remarks>
    /// Callers that need a stable snapshot while the registry may be modified should materialize the sequence before enumerating it.
    /// </remarks>
    public IEnumerable<Type> PacketTypes { get; }
    /// <summary>Gets whether both upgrade and downgrade mapping tables are empty.</summary>
    public bool IsEmpty { get; }

    /// <summary>Determines whether a transformation pipeline exists for a packet type.</summary>
    /// <typeparam name="T">The packet type to query.</typeparam>
    /// <param name="type">The upgrade or downgrade table to query.</param>
    /// <returns><see langword="true" /> when a compatible entry exists; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type" /> is not upgrade or downgrade.</exception>
    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    /// <summary>Determines whether a transformation pipeline exists for a message's runtime type.</summary>
    /// <param name="message">The message to query.</param>
    /// <param name="type">The upgrade or downgrade table to query.</param>
    /// <returns><see langword="true" /> when a compatible entry exists; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type" /> is not upgrade or downgrade.</exception>
    public bool Contains(IMinecraftMessage message, TransformationType type);
    /// <summary>Determines whether a transformation pipeline exists for a runtime packet type.</summary>
    /// <param name="packetType">The type to query.</param>
    /// <param name="transformationType">The upgrade or downgrade table to query.</param>
    /// <returns><see langword="true" /> when a compatible entry exists; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="transformationType" /> is not upgrade or downgrade.</exception>
    public bool Contains(Type packetType, TransformationType transformationType);
    /// <summary>Attempts to get the exact packet type's ordered transformation pipeline.</summary>
    /// <param name="packetType">The exact registered packet type.</param>
    /// <param name="type">The upgrade or downgrade table to query.</param>
    /// <param name="transformation">When successful, the ordered transformation array; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when an exact entry exists; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type" /> is not upgrade or downgrade.</exception>
    public bool TryGetFor(Type packetType, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation);
    /// <summary>Clears existing pipelines and selects transformations applicable beyond a protocol version.</summary>
    /// <param name="mappings">Packet types keyed by their transformation mappings.</param>
    /// <param name="protocolVersion">The current protocol version used to filter mappings.</param>
    /// <returns>This registry.</returns>
    /// <exception cref="ArgumentException">More than one mapping set attempts to register the same packet type.</exception>
    public IMinecraftPacketTransformationsRegistry Replace(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type> mappings, ProtocolVersion protocolVersion);
    /// <summary>Adds ordered upgrade and downgrade pipelines applicable beyond a protocol version.</summary>
    /// <param name="mappings">Packet types keyed by their transformation mappings.</param>
    /// <param name="protocolVersion">The current protocol version used to filter mappings.</param>
    /// <returns>This registry.</returns>
    /// <exception cref="ArgumentException">The packet type already has a registered pipeline.</exception>
    public IMinecraftPacketTransformationsRegistry Add(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type> mappings, ProtocolVersion protocolVersion);
    /// <summary>Removes every upgrade and downgrade pipeline.</summary>
    public void Clear();
    /// <summary>Removes pipelines for packet types belonging to a protocol direction.</summary>
    /// <param name="direction">The clientbound or serverbound direction to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="direction" /> is not clientbound or serverbound.</exception>
    public void Clear(Direction direction);
}
