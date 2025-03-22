using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

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
        { [new PacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginPluginResponsePacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, ProtocolVersion.MINECRAFT_1_19_4)], typeof(BundleDelimiterPacket) },
        {
            [
                new PacketMapping(0x1B, Plugin.SupportedVersions.First()),
                new PacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_14),
                new PacketMapping(0x1B, ProtocolVersion.MINECRAFT_1_15),
                new PacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_16),
                new PacketMapping(0x19, ProtocolVersion.MINECRAFT_1_16_2),
                new PacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_17),
                new PacketMapping(0x17, ProtocolVersion.MINECRAFT_1_19),
                new PacketMapping(0x19, ProtocolVersion.MINECRAFT_1_19_1),
                new PacketMapping(0x17, ProtocolVersion.MINECRAFT_1_19_3),
                new PacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_19_4),
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new PacketMapping(0x25, Plugin.SupportedVersions.First()),
                new PacketMapping(0x25, ProtocolVersion.MINECRAFT_1_14),
                new PacketMapping(0x26, ProtocolVersion.MINECRAFT_1_15),
                new PacketMapping(0x25, ProtocolVersion.MINECRAFT_1_16),
                new PacketMapping(0x24, ProtocolVersion.MINECRAFT_1_16_2),
                new PacketMapping(0x26, ProtocolVersion.MINECRAFT_1_17),
                new PacketMapping(0x23, ProtocolVersion.MINECRAFT_1_19),
                new PacketMapping(0x25, ProtocolVersion.MINECRAFT_1_19_1),
                new PacketMapping(0x24, ProtocolVersion.MINECRAFT_1_19_3),
                new PacketMapping(0x28, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(JoinGamePacket)
        },
        {
            [
                new PacketMapping(0x38, Plugin.SupportedVersions.First()),
                new PacketMapping(0x3A, ProtocolVersion.MINECRAFT_1_14),
                new PacketMapping(0x3B, ProtocolVersion.MINECRAFT_1_15),
                new PacketMapping(0x3A, ProtocolVersion.MINECRAFT_1_16),
                new PacketMapping(0x39, ProtocolVersion.MINECRAFT_1_16_2),
                new PacketMapping(0x3D, ProtocolVersion.MINECRAFT_1_17),
                new PacketMapping(0x3B, ProtocolVersion.MINECRAFT_1_19),
                new PacketMapping(0x3E, ProtocolVersion.MINECRAFT_1_19_1),
                new PacketMapping(0x3D, ProtocolVersion.MINECRAFT_1_19_3),
                new PacketMapping(0x41, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(RespawnPacket)
        }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        {
            [
                new PacketMapping(0x02, Plugin.SupportedVersions.First()),
                new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_14, ProtocolVersion.MINECRAFT_1_18_2)
            ],
            typeof(ChatMessagePacket)
        },
        {
            [
                new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_19),
                new PacketMapping(0x04, ProtocolVersion.MINECRAFT_1_19_1, ProtocolVersion.MINECRAFT_1_19_1)
            ],
            typeof(KeyedChatCommandPacket)
        },
        {
            [
                new PacketMapping(0x04, ProtocolVersion.MINECRAFT_1_19_3)
            ],
            typeof(SignedChatCommandPacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}