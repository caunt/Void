using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;

namespace Void.Minecraft.Network.Registries.PacketId;

/// <summary>
/// Maintains bidirectional packet type-to-identifier mappings for one protocol version and operation.
/// </summary>
public interface IMinecraftPacketIdRegistry
{
    /// <summary>Gets the packet types currently mapped by numeric identifier.</summary>
    public IEnumerable<Type> PacketTypes { get; }
    /// <summary>Gets whether both forward and reverse mapping tables contain no entries.</summary>
    public bool IsEmpty { get; }

    /// <summary>Determines whether a compatible mapping exists for a packet type.</summary>
    /// <typeparam name="T">The packet type to query.</typeparam>
    /// <returns><see langword="true" /> when a compatible type is registered; otherwise, <see langword="false" />.</returns>
    public bool Contains<T>() where T : IMinecraftPacket;
    /// <summary>Determines whether a compatible mapping exists for a message's runtime type.</summary>
    /// <param name="message">The message to query.</param>
    /// <returns><see langword="true" /> when a compatible type is registered; otherwise, <see langword="false" />.</returns>
    public bool Contains(INetworkMessage message);
    /// <summary>Determines whether a compatible mapping exists for a runtime type.</summary>
    /// <param name="type">The type to query.</param>
    /// <returns><see langword="true" /> when a compatible type is registered; otherwise, <see langword="false" />.</returns>
    public bool Contains(Type type);
    /// <summary>Attempts to create the registered packet type's static decoder for an identifier.</summary>
    /// <param name="id">The packet identifier.</param>
    /// <param name="packet">When successful, the decoder delegate; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when the identifier maps to a type with a compatible public static decode method; otherwise, <see langword="false" />.</returns>
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet);
    /// <summary>Attempts to resolve both the packet type and its static decoder for an identifier.</summary>
    /// <param name="id">The packet identifier.</param>
    /// <param name="packetType">When successful, the registered packet type; otherwise, <see langword="null" />.</param>
    /// <param name="packet">When successful, the decoder delegate; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when both type and decoder are available; otherwise, <see langword="false" />.</returns>
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out Type packetType, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet);
    /// <summary>Attempts to get the identifier registered for a packet's exact runtime type.</summary>
    /// <param name="packet">The packet instance to query.</param>
    /// <param name="id">When successful, the registered identifier; otherwise, the default integer value.</param>
    /// <returns><see langword="true" /> when the runtime type has a reverse mapping; otherwise, <see langword="false" />.</returns>
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    /// <summary>Attempts to get the type mapped to a packet identifier.</summary>
    /// <param name="id">The packet identifier.</param>
    /// <param name="packetType">When successful, the mapped type; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when the identifier is registered; otherwise, <see langword="false" />.</returns>
    public bool TryGetType(int id, [MaybeNullWhen(false)] out Type packetType);
    /// <summary>Clears existing entries and selects mappings valid for a protocol version.</summary>
    /// <param name="mappings">Packet types keyed by ordered identifier mapping arrays.</param>
    /// <param name="protocolVersion">The protocol version for which entries are selected.</param>
    /// <returns>This registry.</returns>
    /// <exception cref="ArgumentException">A mapping interval is reversed or a selected identifier or type conflicts with an existing selected entry.</exception>
    public IMinecraftPacketIdRegistry ReplacePackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings, ProtocolVersion protocolVersion);
    /// <summary>Adds mappings valid for a protocol version without clearing existing entries.</summary>
    /// <param name="mappings">Packet types keyed by ordered identifier mapping arrays.</param>
    /// <param name="protocolVersion">The protocol version for which entries are selected.</param>
    /// <returns>This registry.</returns>
    /// <exception cref="ArgumentException">A mapping interval is reversed or a selected identifier or type conflicts with an existing entry.</exception>
    public IMinecraftPacketIdRegistry AddPackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings, ProtocolVersion protocolVersion);
    /// <summary>Removes every forward and reverse packet mapping.</summary>
    public void Clear();
    /// <summary>Removes mappings whose packet types belong to a protocol direction.</summary>
    /// <param name="direction">The clientbound or serverbound direction to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="direction" /> is not clientbound or serverbound.</exception>
    public void Clear(Direction direction);
}
