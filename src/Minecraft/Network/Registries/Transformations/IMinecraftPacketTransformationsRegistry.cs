using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Network;

namespace Void.Minecraft.Network.Registries.Transformations;

public interface IMinecraftPacketTransformationsRegistry
{
    /// <summary>
    /// Gets the packet <see cref="Type"/> values that currently have transformation entries in this registry.
    /// </summary>
    /// <remarks>
    /// Callers that need a stable snapshot while the registry may be modified should materialize the sequence before enumerating it.
    /// </remarks>
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public bool TryGetFor(Type packetType, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation);
    public IMinecraftPacketTransformationsRegistry Replace(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketTransformationsRegistry Add(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
    public void Clear(Direction direction);
}
