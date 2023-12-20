using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Network.Protocol.Packets.Serverbound;
using Void.Proxy.Network.Protocol.Packets.Shared;

namespace Void.Proxy.Network.Protocol.Registry;

public static class Registries
{
    public static readonly StateRegistry HandshakeStateRegistry = new();
    public static readonly StateRegistry LoginStateRegistry = new();
    public static readonly StateRegistry ConfigurationStateRegistry = new();
    public static readonly StateRegistry PlayStateRegistry = new();

    public static void Fill()
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

        ConfigurationStateRegistry.Serverbound.Register<PluginMessage>(() => new(Direction.Serverbound, 32767), new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_20_2));
        ConfigurationStateRegistry.Clientbound.Register<PluginMessage>(() => new(Direction.Clientbound, 1048576), new PacketMapping(0x00, false, ProtocolVersion.MINECRAFT_1_20_2));
        ConfigurationStateRegistry.Clientbound.Register<DisconnectPacket>(() => new(), new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_20_2));
        ConfigurationStateRegistry.Clientbound.Register<FinishConfiguration>(() => new(), new PacketMapping(0x02, false, ProtocolVersion.MINECRAFT_1_20_2));

        // not used - PlayStateRegistry.Serverbound.Register<PlayerSessionPacket>(() => new(), new PacketMapping(0x06, false, ProtocolVersion.MINECRAFT_1_7_2));
        PlayStateRegistry.Clientbound.Register<StartConfiguration>(() => new(),
            new PacketMapping(0x65, false, ProtocolVersion.MINECRAFT_1_20_2),
            new PacketMapping(0x67, false, ProtocolVersion.MINECRAFT_1_20_3));
        PlayStateRegistry.Serverbound.Register<AcknowledgeConfiguration>(() => new(),
            new PacketMapping(0x0B, false, ProtocolVersion.MINECRAFT_1_20_2));
        PlayStateRegistry.Serverbound.Register<PluginMessage>(() => new(Direction.Serverbound, 32767),
            new PacketMapping(0x17, false, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x09, false, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x0A, false, ProtocolVersion.MINECRAFT_1_12),
            new PacketMapping(0x09, false, ProtocolVersion.MINECRAFT_1_12_1),
            new PacketMapping(0x0A, false, ProtocolVersion.MINECRAFT_1_13),
            new PacketMapping(0x0B, false, ProtocolVersion.MINECRAFT_1_14),
            new PacketMapping(0x0A, false, ProtocolVersion.MINECRAFT_1_17),
            new PacketMapping(0x0C, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x0D, false, ProtocolVersion.MINECRAFT_1_19_1),
            new PacketMapping(0x0C, false, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x0D, false, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x0F, false, ProtocolVersion.MINECRAFT_1_20_2));
        PlayStateRegistry.Serverbound.Register<ChatMessage>(() => new(Direction.Serverbound),
            new PacketMapping(0x01, false, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x02, false, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x03, false, ProtocolVersion.MINECRAFT_1_12),
            new PacketMapping(0x02, false, ProtocolVersion.MINECRAFT_1_12_1),
            new PacketMapping(0x03, false, ProtocolVersion.MINECRAFT_1_14, ProtocolVersion.MINECRAFT_1_18_2));
        PlayStateRegistry.Serverbound.Register<KeyedChatMessage>(() => new(),
            new PacketMapping(0x04, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x05, false, ProtocolVersion.MINECRAFT_1_19_1, ProtocolVersion.MINECRAFT_1_19_1));
        PlayStateRegistry.Serverbound.Register<SessionChatMessage>(() => new(),
            new PacketMapping(0x05, false, ProtocolVersion.MINECRAFT_1_19_3, ProtocolVersion.MINECRAFT_1_20_3));
        PlayStateRegistry.Serverbound.Register<KeyedChatCommand>(() => new(),
            new PacketMapping(0x03, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x04, false, ProtocolVersion.MINECRAFT_1_19_1, ProtocolVersion.MINECRAFT_1_19_1));
        PlayStateRegistry.Serverbound.Register<SessionChatCommand>(() => new(),
            new PacketMapping(0x04, false, ProtocolVersion.MINECRAFT_1_19_3));
        PlayStateRegistry.Clientbound.Register<ChatMessage>(() => new(Direction.Clientbound),
            new PacketMapping(0x02, true, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x0F, true, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x0E, true, ProtocolVersion.MINECRAFT_1_13),
            new PacketMapping(0x0F, true, ProtocolVersion.MINECRAFT_1_15),
            new PacketMapping(0x0E, true, ProtocolVersion.MINECRAFT_1_16),
            new PacketMapping(0x0F, true, ProtocolVersion.MINECRAFT_1_17, ProtocolVersion.MINECRAFT_1_18_2));
        PlayStateRegistry.Clientbound.Register<SystemChatMessage>(() => new(),
            new PacketMapping(0x5F, true, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x62, true, ProtocolVersion.MINECRAFT_1_19_1),
            new PacketMapping(0x60, true, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x64, true, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x67, true, ProtocolVersion.MINECRAFT_1_20_2),
            new PacketMapping(0x69, true, ProtocolVersion.MINECRAFT_1_20_3));
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
        PlayStateRegistry.Clientbound.Register<RespawnPacket>(() => new(),
            new PacketMapping(0x07, false, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x33, false, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x34, false, ProtocolVersion.MINECRAFT_1_12),
            new PacketMapping(0x35, false, ProtocolVersion.MINECRAFT_1_12_1),
            new PacketMapping(0x38, false, ProtocolVersion.MINECRAFT_1_13),
            new PacketMapping(0x3A, false, ProtocolVersion.MINECRAFT_1_14),
            new PacketMapping(0x3B, false, ProtocolVersion.MINECRAFT_1_15),
            new PacketMapping(0x3A, false, ProtocolVersion.MINECRAFT_1_16),
            new PacketMapping(0x39, false, ProtocolVersion.MINECRAFT_1_16_2),
            new PacketMapping(0x3D, false, ProtocolVersion.MINECRAFT_1_17),
            new PacketMapping(0x3B, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x3E, false, ProtocolVersion.MINECRAFT_1_19_1),
            new PacketMapping(0x3D, false, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x41, false, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x43, false, ProtocolVersion.MINECRAFT_1_20_2));
        PlayStateRegistry.Clientbound.Register<PluginMessage>(() => new(Direction.Clientbound, 1048576),
            new PacketMapping(0x3F, false, ProtocolVersion.MINECRAFT_1_7_2),
            new PacketMapping(0x18, false, ProtocolVersion.MINECRAFT_1_9),
            new PacketMapping(0x19, false, ProtocolVersion.MINECRAFT_1_13),
            new PacketMapping(0x18, false, ProtocolVersion.MINECRAFT_1_14),
            new PacketMapping(0x19, false, ProtocolVersion.MINECRAFT_1_15),
            new PacketMapping(0x18, false, ProtocolVersion.MINECRAFT_1_16),
            new PacketMapping(0x17, false, ProtocolVersion.MINECRAFT_1_16_2),
            new PacketMapping(0x18, false, ProtocolVersion.MINECRAFT_1_17),
            new PacketMapping(0x15, false, ProtocolVersion.MINECRAFT_1_19),
            new PacketMapping(0x16, false, ProtocolVersion.MINECRAFT_1_19_1),
            new PacketMapping(0x15, false, ProtocolVersion.MINECRAFT_1_19_3),
            new PacketMapping(0x17, false, ProtocolVersion.MINECRAFT_1_19_4),
            new PacketMapping(0x18, false, ProtocolVersion.MINECRAFT_1_20_2));
    }
}
