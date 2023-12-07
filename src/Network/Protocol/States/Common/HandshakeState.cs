﻿using MinecraftProxy.Network.Protocol.Packets.Serverbound;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class HandshakeState(Player player) : ProtocolState
{
    protected override Dictionary<int, Type> serverboundPackets => new()
    {
        { 0x00, typeof(HandshakePacket) }
    };

    protected override Dictionary<int, Type> clientboundPackets => new()
    {
    };

    public Task<bool> HandleAsync(HandshakePacket packet)
    {
        player.SetProtocolVersion(ProtocolVersion.Get(packet.ProtocolVersion));
        player.SwitchState(packet.NextState);
        return Task.FromResult(false);
    }
}