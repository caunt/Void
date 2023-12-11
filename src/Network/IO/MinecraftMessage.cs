﻿using MinecraftProxy.Network.Protocol;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.States;
using MinecraftProxy.Network.Protocol.States.Common;
using System.Buffers;
using System.Text.Json;

namespace MinecraftProxy.Network.IO;

public readonly struct MinecraftMessage(int packetId, Memory<byte> memory, IMemoryOwner<byte> owner) : IDisposable
{
    public int PacketId { get; } = packetId;
    public int Length { get; } = memory.Length;
    public Memory<byte> Memory { get; } = memory;
    public void Dispose() => owner.Dispose();

    public (int, IMinecraftPacket?, Task<bool>) DecodeAndHandle(ProtocolState protocolState, Direction direction, ProtocolVersion protocolVersion)
    {
        var buffer = new MinecraftBuffer(Memory);
        Proxy.Logger.Verbose($"Decoding {direction} 0x{packetId:X2} packet");

        try
        {
            return protocolState switch
            {
                HandshakeState state when state.Decode<HandshakeState>(packetId, direction, ref buffer, protocolVersion) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                LoginState state when state.Decode<LoginState>(packetId, direction, ref buffer, protocolVersion) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                ConfigurationState state when state.Decode<ConfigurationState>(packetId, direction, ref buffer, protocolVersion) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                PlayState state when state.Decode<PlayState>(packetId, direction, ref buffer, protocolVersion) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                _ => (packetId, null, Task.FromResult(false))
            };
        }
        catch (Exception exception)
        {
            Proxy.Logger.Information($"Couldn't decode packet: {exception}");
            return (packetId, null, Task.FromResult(false));
        }
    }

    public static MinecraftMessage Encode(int packetId, IMinecraftPacket packet, Direction direction, ProtocolVersion protocolVersion) // TODO make size calculations in interface
    {
        var memoryOwner = MemoryPool<byte>.Shared.Rent(packet.MaxSize());
        var buffer = new MinecraftBuffer(memoryOwner.Memory);
        Proxy.Logger.Verbose($"Encoding {direction} 0x{packetId:X2} packet {JsonSerializer.Serialize(packet as object, Proxy.JsonSerializerOptions)}");

        packet.Encode(ref buffer, protocolVersion);

        return new(packetId, memoryOwner.Memory[..buffer.Position], memoryOwner);
    }
}