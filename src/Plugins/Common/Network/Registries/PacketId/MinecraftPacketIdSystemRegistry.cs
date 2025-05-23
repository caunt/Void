﻿using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.Registries.PacketId;

public class MinecraftPacketIdSystemRegistry : IMinecraftPacketIdSystemRegistry
{
    public bool IsEmpty => this is { Read.IsEmpty: true, Write.IsEmpty: true, ManagedBy: null };
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketIdRegistry Read { get; set; } = new MinecraftPacketIdRegistry();
    public IMinecraftPacketIdRegistry Write { get; set; } = new MinecraftPacketIdRegistry();

    public bool Contains<T>() where T : IMinecraftMessage
    {
        return Contains(typeof(T));
    }

    public bool Contains(IMinecraftMessage packet)
    {
        return Contains(packet.GetType());
    }

    public bool Contains(Type type)
    {
        return Read.Contains(type) || Write.Contains(type);
    }

    public void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        switch (operation)
        {
            case Operation.Read:
                Read.ReplacePackets(mappings, ProtocolVersion);
                break;
            case Operation.Write:
                Write.ReplacePackets(mappings, ProtocolVersion);
                break;
            case Operation.Any:
                Read.ReplacePackets(mappings, ProtocolVersion);
                Write.ReplacePackets(mappings, ProtocolVersion);
                break;
            default:
                throw new ArgumentException($"Invalid operation {operation}", nameof(operation));
        }
    }

    public void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        if (ProtocolVersion is null)
            throw new InvalidOperationException("Protocol version is not set yet");

        switch (operation)
        {
            case Operation.Read:
                Read.AddPackets(mappings, ProtocolVersion);
                break;
            case Operation.Write:
                Write.AddPackets(mappings, ProtocolVersion);
                break;
            case Operation.Any:
                Read.AddPackets(mappings, ProtocolVersion);
                Write.AddPackets(mappings, ProtocolVersion);
                break;
            default:
                throw new ArgumentException($"Invalid operation {operation}", nameof(operation));
        }
    }

    public void Reset()
    {
        ProtocolVersion = null;
        ManagedBy = null;

        Read.Clear();
        Write.Clear();
    }
}
