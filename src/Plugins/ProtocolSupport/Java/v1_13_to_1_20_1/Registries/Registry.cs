using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

public static class Registry
{
    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundHandshakeMappings = new Dictionary<MinecraftPacketIdMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundHandshakeMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { [new MinecraftPacketIdMapping(0x00, Plugin.SupportedVersions.First())], typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundStatusMappings = new Dictionary<MinecraftPacketIdMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundStatusMappings = new Dictionary<MinecraftPacketIdMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundLoginMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { [new MinecraftPacketIdMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginDisconnectPacket) },
        { [new MinecraftPacketIdMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionRequestPacket) },
        { [new MinecraftPacketIdMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginSuccessPacket) },
        { [new MinecraftPacketIdMapping(0x03, Plugin.SupportedVersions.First())], typeof(SetCompressionPacket) },
        { [new MinecraftPacketIdMapping(0x04, Plugin.SupportedVersions.First())], typeof(LoginPluginRequestPacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundLoginMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { [new MinecraftPacketIdMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginStartPacket) },
        { [new MinecraftPacketIdMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionResponsePacket) },
        { [new MinecraftPacketIdMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginPluginResponsePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { [new MinecraftPacketIdMapping(0x00, ProtocolVersion.MINECRAFT_1_19_4)], typeof(BundleDelimiterPacket) },
        {
            [
                new MinecraftPacketIdMapping(0x1B, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x1A, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketIdMapping(0x1B, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketIdMapping(0x1A, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketIdMapping(0x19, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketIdMapping(0x1A, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketIdMapping(0x17, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x19, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketIdMapping(0x17, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketIdMapping(0x1A, ProtocolVersion.MINECRAFT_1_19_4),
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x21, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x20, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketIdMapping(0x21, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketIdMapping(0x20, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketIdMapping(0x1F, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketIdMapping(0x21, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketIdMapping(0x1E, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x20, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketIdMapping(0x1F, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketIdMapping(0x23, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(KeepAliveRequestPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x25, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x25, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketIdMapping(0x26, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketIdMapping(0x25, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketIdMapping(0x24, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketIdMapping(0x26, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketIdMapping(0x23, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x25, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketIdMapping(0x24, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketIdMapping(0x28, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(JoinGamePacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x38, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x3A, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketIdMapping(0x3B, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketIdMapping(0x3A, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketIdMapping(0x39, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketIdMapping(0x3D, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketIdMapping(0x3B, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x3E, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketIdMapping(0x3D, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketIdMapping(0x41, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(RespawnPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x5F, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x62, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketIdMapping(0x60, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketIdMapping(0x64, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(SystemChatMessagePacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x0E, ProtocolVersion.MINECRAFT_1_13),
                new MinecraftPacketIdMapping(0x0F, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketIdMapping(0x0E, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketIdMapping(0x0F, ProtocolVersion.MINECRAFT_1_17, ProtocolVersion.MINECRAFT_1_18_2)
            ],
            typeof(Common.Network.Packets.Clientbound.ChatMessagePacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        {
            [
                new MinecraftPacketIdMapping(0x0E, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x0F, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketIdMapping(0x10, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketIdMapping(0x0F, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketIdMapping(0x11, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x12, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketIdMapping(0x11, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketIdMapping(0x12, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(KeepAliveResponsePacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x02, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x03, ProtocolVersion.MINECRAFT_1_14, ProtocolVersion.MINECRAFT_1_18_2)
            ],
            typeof(Packets.Serverbound.ChatMessagePacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x03, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketIdMapping(0x04, ProtocolVersion.MINECRAFT_1_19_1, ProtocolVersion.MINECRAFT_1_19_1)
            ],
            typeof(KeyedChatCommandPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x04, ProtocolVersion.MINECRAFT_1_19_3)
            ],
            typeof(SignedChatCommandPacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}
