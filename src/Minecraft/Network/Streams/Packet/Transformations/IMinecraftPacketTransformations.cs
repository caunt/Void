using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Streams.Packet.Transformations;

public interface IMinecraftPacketTransformations
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public bool TryGetTransformation(Type packetType, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation);
    public IMinecraftPacketTransformations ReplaceTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketTransformations AddTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}
