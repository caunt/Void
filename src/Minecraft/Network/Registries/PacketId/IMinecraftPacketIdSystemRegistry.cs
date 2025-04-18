﻿using System;
using System.Collections.Generic;
using Void.Common.Network;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.PacketId;

public interface IMinecraftPacketIdSystemRegistry
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketIdRegistry Read { get; set; }
    public IMinecraftPacketIdRegistry Write { get; set; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(Type type);
    public bool Contains(IMinecraftMessage packet);
    public void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings);
    public void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings);
    public void Reset();
}
