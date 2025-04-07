﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Common.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Streams.Packet.Registries;

public interface IMinecraftPacketIdRegistry
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(INetworkMessage message);
    public bool Contains(Type type);
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet);
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out Type packetType, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public bool TryGetType(int id, [MaybeNullWhen(false)] out Type packetType);
    public IMinecraftPacketIdRegistry ReplacePackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketIdRegistry AddPackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}
