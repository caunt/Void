using System;
using System.Collections.Generic;
using Void.Common.Plugins;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Streams.Packet.Transformations;

public interface IMinecraftPacketPluginsTransformations
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketTransformations> All { get; }

    public IMinecraftPacketTransformations Get(IPlugin plugin);
    public void Remove(IPlugin plugin);
    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public void Clear();
    public void Reset();
}
