using System;
using System.Collections.Generic;
using Void.Common.Network;
using Void.Common.Plugins;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Streams.Packet.Registries;

public interface IMinecraftPacketSystemRegistry
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
