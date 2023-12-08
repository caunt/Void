﻿using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;

namespace MinecraftProxy.Network.Protocol.Registry;

public static class Registries
{
    public static readonly StateRegistry HandshakeStateRegistry = new();
    public static readonly StateRegistry LoginStateRegistry = new();
    public static readonly StateRegistry ConfigurationStateRegistry = new();
    public static readonly StateRegistry PlayStateRegistry = new();

    static Registries()
    {
        HandshakeStateRegistry.Serverbound.Register<HandshakePacket>(() => new(), new PacketMapping(0x00, false, ProtocolVersion.MINECRAFT_1_7_2));

        LoginStateRegistry.Serverbound.Register<LoginStartPacket>(() => new(), new PacketMapping(0x00, false, ProtocolVersion.MINECRAFT_1_7_2));
        LoginStateRegistry.Serverbound.Register<EncryptionResponsePacket>(() => new(), new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_7_2));
        LoginStateRegistry.Serverbound.Register<LoginPluginResponse>(() => new(), new PacketMapping(0x02, false, ProtocolVersion.MINECRAFT_1_13));
        LoginStateRegistry.Serverbound.Register<LoginAcknowledgedPacket>(() => new(), new PacketMapping(0x03, false, ProtocolVersion.MINECRAFT_1_20_2));
        LoginStateRegistry.Clientbound.Register<DisconnectPacket>(() => new(false), new PacketMapping(0x00, false, ProtocolVersion.MINECRAFT_1_7_2));
        LoginStateRegistry.Clientbound.Register<EncryptionRequestPacket>(() => new(), new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_7_2));
        LoginStateRegistry.Clientbound.Register<LoginSuccessPacket>(() => new(), new PacketMapping(0x02, false, ProtocolVersion.MINECRAFT_1_7_2));
        LoginStateRegistry.Clientbound.Register<SetCompressionPacket>(() => new(), new PacketMapping(0x03, false, ProtocolVersion.MINECRAFT_1_8));
        LoginStateRegistry.Clientbound.Register<LoginPluginRequest>(() => new(), new PacketMapping(0x04, false, ProtocolVersion.MINECRAFT_1_13));

        ConfigurationStateRegistry.Clientbound.Register<DisconnectPacket>(() => new(), new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_20_2));
        ConfigurationStateRegistry.Clientbound.Register<FinishConfiguration>(() => new(), new PacketMapping(0x02, false, ProtocolVersion.MINECRAFT_1_20_2));

        // not used - PlayStateRegistry.Serverbound.Register<PlayerSessionPacket>(() => new(), new PacketMapping(0x06, false, ProtocolVersion.MINECRAFT_1_7_2));
        PlayStateRegistry.Clientbound.Register<DisconnectPacket>(() => new(),
            new PacketMapping(0x40, false, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x1A, false, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x1B, false, ProtocolVersion.MINECRAFT_1_13),
            new PacketMapping(0x1A, false, ProtocolVersion.MINECRAFT_1_14),
            new PacketMapping(0x1B, false, ProtocolVersion.MINECRAFT_1_15),
            new PacketMapping(0x1A, false, ProtocolVersion.MINECRAFT_1_16),
            new PacketMapping(0x19, false, ProtocolVersion.MINECRAFT_1_16_2),
            new PacketMapping(0x1A, false, ProtocolVersion.MINECRAFT_1_17),
            new PacketMapping(0x17, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x19, false, ProtocolVersion.MINECRAFT_1_19_1),
            new PacketMapping(0x17, false, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x1A, false, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x1B, false, ProtocolVersion.MINECRAFT_1_20_2));
        PlayStateRegistry.Clientbound.Register<PlayerInfoUpdatePacket>(() => new(),
            new PacketMapping(0x36, false, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x3A, false, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x3C, false, ProtocolVersion.MINECRAFT_1_20_2));
        PlayStateRegistry.Clientbound.Register<JoinGamePacket>(() => new(),
            new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x23, false, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x25, false, ProtocolVersion.MINECRAFT_1_13),
            new PacketMapping(0x25, false, ProtocolVersion.MINECRAFT_1_14),
            new PacketMapping(0x26, false, ProtocolVersion.MINECRAFT_1_15),
            new PacketMapping(0x25, false, ProtocolVersion.MINECRAFT_1_16),
            new PacketMapping(0x24, false, ProtocolVersion.MINECRAFT_1_16_2),
            new PacketMapping(0x26, false, ProtocolVersion.MINECRAFT_1_17),
            new PacketMapping(0x23, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x25, false, ProtocolVersion.MINECRAFT_1_19_1),
            new PacketMapping(0x24, false, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x28, false, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x29, false, ProtocolVersion.MINECRAFT_1_20_2));
    }
}