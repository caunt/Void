using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public static class Registry
{
    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ClientboundHandshakeMappings = new Dictionary<MinecraftPacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundHandshakeMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        { [new MinecraftPacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ClientboundStatusMappings = new Dictionary<MinecraftPacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundStatusMappings = new Dictionary<MinecraftPacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ClientboundLoginMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        { [new MinecraftPacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginDisconnectPacket) },
        { [new MinecraftPacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionRequestPacket) },
        { [new MinecraftPacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginSuccessPacket) },
        { [new MinecraftPacketMapping(0x03, Plugin.SupportedVersions.First())], typeof(SetCompressionPacket) },
        { [new MinecraftPacketMapping(0x04, Plugin.SupportedVersions.First())], typeof(LoginPluginRequestPacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundLoginMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        { [new MinecraftPacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginStartPacket) },
        { [new MinecraftPacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionResponsePacket) },
        { [new MinecraftPacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginPluginResponsePacket) },
        { [new MinecraftPacketMapping(0x03, Plugin.SupportedVersions.First())], typeof(LoginAcknowledgedPacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ClientboundConfigurationMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        {
            [
                new MinecraftPacketMapping(0x01, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x02, ProtocolVersion.MINECRAFT_1_20_5)
            ], typeof(ConfigurationDisconnectPacket)
        },
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundConfigurationMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        {
            [
                new MinecraftPacketMapping(0x02, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x03, ProtocolVersion.MINECRAFT_1_20_5)
            ], typeof(AcknowledgeFinishConfigurationPacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ClientboundPlayMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        { [new MinecraftPacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(BundleDelimiterPacket) },
        {
            [
                new MinecraftPacketMapping(0x24, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x26, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x27, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(KeepAliveRequestPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x65, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x67, ProtocolVersion.MINECRAFT_1_20_3),
                new MinecraftPacketMapping(0x69, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x70, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(StartConfigurationPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x1B, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x1D, ProtocolVersion.MINECRAFT_1_20_5)
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x67, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x69, ProtocolVersion.MINECRAFT_1_20_3),
                new MinecraftPacketMapping(0x6C, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x73, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(SystemChatMessagePacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        {
            [
                new MinecraftPacketMapping(0x04, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x05, ProtocolVersion.MINECRAFT_1_21_2)
            ], typeof(ChatCommandPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x04, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x05, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x06, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(SignedChatCommandPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x14, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x15, ProtocolVersion.MINECRAFT_1_20_3),
                new MinecraftPacketMapping(0x18, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(KeepAliveResponsePacket)
        },
        {
            [
                new MinecraftPacketMapping(0x0B, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x0C, ProtocolVersion.MINECRAFT_1_20_5),
                new MinecraftPacketMapping(0x0E, ProtocolVersion.MINECRAFT_1_21_4)
            ],
            typeof(AcknowledgeConfigurationPacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}