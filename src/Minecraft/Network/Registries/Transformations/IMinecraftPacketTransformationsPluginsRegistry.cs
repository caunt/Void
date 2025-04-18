﻿using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.Transformations;

public interface IMinecraftPacketTransformationsPluginsRegistry
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketTransformationsRegistry> All { get; }

    public IMinecraftPacketTransformationsRegistry Get(IPlugin plugin);
    public void Remove(IPlugin plugin);
    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public void Clear();
    public void Reset();
}
