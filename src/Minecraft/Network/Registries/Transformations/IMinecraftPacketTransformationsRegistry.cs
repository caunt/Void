using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Minecraft.Network.Registries.Transformations;

public interface IMinecraftPacketTransformationsRegistry
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public bool TryGetTransformations(Type packetType, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation);
    public IMinecraftPacketTransformationsRegistry ReplaceTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketTransformationsRegistry AddTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}
