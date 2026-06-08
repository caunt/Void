using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.PacketId;

public interface IMinecraftPacketIdPluginsRegistry
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> Read { get; }
    public IReadOnlyCollection<IMinecraftPacketIdRegistry> Write { get; }

    public IMinecraftPacketIdRegistry Get(Operation operation, IPlugin plugin);
    public bool TryGetPlugin<T>([MaybeNullWhen(false)] out IPlugin plugin) where T : IMinecraftPacket;
    public bool TryGetPlugin(INetworkMessage message, [MaybeNullWhen(false)] out IPlugin plugin);
    public bool TryGetPlugin(Type type, [MaybeNullWhen(false)] out IPlugin plugin);
    public void Remove(IPlugin plugin);
    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(INetworkMessage message);
    public bool Contains(Type type);
    public void Clear();
    /// <summary>
    /// Clears packet ID registrations for the specified direction from the selected operation buckets.
    /// </summary>
    /// <remarks>
    /// When <paramref name="operation"/> includes <see cref="Operation.Read"/>, each read registry is cleared for
    /// <paramref name="direction"/>. When it includes <see cref="Operation.Write"/>, each write registry is cleared.
    /// If neither flag is present, this method has no effect.
    /// </remarks>
    /// <param name="direction">The packet direction whose registrations should be removed.</param>
    /// <param name="operation">The read and/or write registry buckets to clear.</param>
    public void Clear(Direction direction, Operation operation);
    public void Reset();
}
