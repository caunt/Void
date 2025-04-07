using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Common.Network.Messages;
using Void.Common.Plugins;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Streams.Packet.Registries;

public interface IMinecraftPacketPluginsRegistry
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> All { get; }

    public IMinecraftPacketIdRegistry Get(IPlugin plugin);
    public bool TryGetPlugin<T>([MaybeNullWhen(false)] out IPlugin plugin) where T : IMinecraftPacket;
    public bool TryGetPlugin(INetworkMessage message, [MaybeNullWhen(false)] out IPlugin plugin);
    public bool TryGetPlugin(Type type, [MaybeNullWhen(false)] out IPlugin plugin);
    public void Remove(IPlugin plugin);
    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(INetworkMessage message);
    public bool Contains(Type type);
    public void Clear();
    public void Reset();
}
