using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public static class Registry
{
    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundHandshakeMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundHandshakeMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundStatusMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundStatusMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginDisconnectPacket) },
        { [new PacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionRequestPacket) },
        { [new PacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginSuccessPacket) },
        { [new PacketMapping(0x03, Plugin.SupportedVersions.First())], typeof(SetCompressionPacket) },
        { [new PacketMapping(0x04, Plugin.SupportedVersions.First())], typeof(LoginPluginRequestPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginStartPacket) },
        { [new PacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionResponsePacket) },
        { [new PacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginPluginResponsePacket) },
        { [new PacketMapping(0x03, Plugin.SupportedVersions.First())], typeof(LoginAcknowledgedPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundConfigurationMappings = new Dictionary<PacketMapping[], Type>
    {
        {
            [
                new PacketMapping(0x01, Plugin.SupportedVersions.First()),
                new PacketMapping(0x02, ProtocolVersion.MINECRAFT_1_20_5)
            ], typeof(ConfigurationDisconnectPacket)
        },
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundConfigurationMappings = new Dictionary<PacketMapping[], Type>
    {
        {
            [
                new PacketMapping(0x02, Plugin.SupportedVersions.First()),
                new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_20_5)
            ], typeof(AcknowledgeFinishConfigurationPacket)
        }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(BundleDelimiterPacket) },
        {
            [
                new PacketMapping(0x24, Plugin.SupportedVersions.First()),
                new PacketMapping(0x26, ProtocolVersion.MINECRAFT_1_20_5),
                new PacketMapping(0x27, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(KeepAliveRequestPacket)
        },
        {
            [
                new PacketMapping(0x65, Plugin.SupportedVersions.First()),
                new PacketMapping(0x67, ProtocolVersion.MINECRAFT_1_20_3),
                new PacketMapping(0x69, ProtocolVersion.MINECRAFT_1_20_5),
                new PacketMapping(0x70, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(StartConfigurationPacket)
        },
        {
            [
                new PacketMapping(0x1B, Plugin.SupportedVersions.First()),
                new PacketMapping(0x1D, ProtocolVersion.MINECRAFT_1_20_5)
            ],
            typeof(PlayDisconnectPacket)
        }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        {
            [
                new PacketMapping(0x04, ProtocolVersion.MINECRAFT_1_20_5),
                new PacketMapping(0x05, ProtocolVersion.MINECRAFT_1_21_2)
            ], typeof(ChatCommandPacket)
        },
        {
            [
                new PacketMapping(0x04, Plugin.SupportedVersions.First()),
                new PacketMapping(0x05, ProtocolVersion.MINECRAFT_1_20_5),
                new PacketMapping(0x06, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(SignedChatCommandPacket)
        },
        {
            [
                new PacketMapping(0x14, Plugin.SupportedVersions.First()),
                new PacketMapping(0x15, ProtocolVersion.MINECRAFT_1_20_3),
                new PacketMapping(0x18, ProtocolVersion.MINECRAFT_1_20_5),
                new PacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_21_2)
            ],
            typeof(KeepAliveResponsePacket)
        },
        {
            [
                new PacketMapping(0x0B, Plugin.SupportedVersions.First()),
                new PacketMapping(0x0C, ProtocolVersion.MINECRAFT_1_20_5),
                new PacketMapping(0x0E, ProtocolVersion.MINECRAFT_1_21_4)
            ],
            typeof(AcknowledgeConfigurationPacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}