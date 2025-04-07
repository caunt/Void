﻿using System;
using Void.Common.Plugins;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;

namespace Void.Minecraft.Network.Registries;

public interface IRegistryHolder : IDisposable
{
    public ProtocolVersion ProtocolVersion { get; }

    public IMinecraftPacketIdSystemRegistry PacketIdSystem { get; }
    public IMinecraftPacketIdPluginsRegistry PacketIdPlugins { get; }
    public IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; }

    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion);
    public void ClearPlugins();
    public string PrintPackets();
    public void DisposeBy(IPlugin managedBy);
}
