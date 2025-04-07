using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

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
        { [new MinecraftPacketIdMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginPluginResponsePacket) },
        { [new MinecraftPacketIdMapping(0x03, Plugin.SupportedVersions.First())], typeof(LoginAcknowledgedPacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundConfigurationMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        {
            [
                new MinecraftPacketIdMapping(0x01, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x02, ProtocolVersion.MINECRAFT_1_20_5)
            ], typeof(ConfigurationDisconnectPacket)
        },
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundConfigurationMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        {
            [
                new MinecraftPacketIdMapping(0x02, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x03, ProtocolVersion.MINECRAFT_1_20_5)
            ], typeof(AcknowledgeFinishConfigurationPacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { [new MinecraftPacketIdMapping(0x00, Plugin.SupportedVersions.First())], typeof(BundleDelimiterPacket) },
        {
            [
                new MinecraftPacketIdMapping(0x24, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x26, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x27, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(KeepAliveRequestPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x65, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x67, ProtocolVersion.MINECRAFT_1_20_3),
                new MinecraftPacketIdMapping(0x69, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x70, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(StartConfigurationPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x1B, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x1D, ProtocolVersion.MINECRAFT_1_20_5)
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x67, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x69, ProtocolVersion.MINECRAFT_1_20_3),
                new MinecraftPacketIdMapping(0x6C, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x73, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(SystemChatMessagePacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        {
            [
                new MinecraftPacketIdMapping(0x04, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x05, ProtocolVersion.MINECRAFT_1_21_2)
            ], typeof(ChatCommandPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x04, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x05, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x06, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(SignedChatCommandPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x14, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x15, ProtocolVersion.MINECRAFT_1_20_3),
                new MinecraftPacketIdMapping(0x18, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x1A, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(KeepAliveResponsePacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x0B, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x0C, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketIdMapping(0x0E, ProtocolVersion.MINECRAFT_1_21_4)
            ],
            typeof(AcknowledgeConfigurationPacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}
