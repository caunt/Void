using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

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
        { [new MinecraftPacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginPluginResponsePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ClientboundPlayMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        { [new MinecraftPacketMapping(0x00, ProtocolVersion.MINECRAFT_1_19_4)], typeof(BundleDelimiterPacket) },
        {
            [
                new MinecraftPacketMapping(0x1B, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketMapping(0x1B, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketMapping(0x19, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketMapping(0x17, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketMapping(0x19, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketMapping(0x17, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_19_4),
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x25, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x25, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketMapping(0x26, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketMapping(0x25, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketMapping(0x24, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketMapping(0x26, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketMapping(0x23, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketMapping(0x25, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketMapping(0x24, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketMapping(0x28, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(JoinGamePacket)
        },
        {
            [
                new MinecraftPacketMapping(0x38, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x3A, ProtocolVersion.MINECRAFT_1_14),
                new MinecraftPacketMapping(0x3B, ProtocolVersion.MINECRAFT_1_15),
                new MinecraftPacketMapping(0x3A, ProtocolVersion.MINECRAFT_1_16),
                new MinecraftPacketMapping(0x39, ProtocolVersion.MINECRAFT_1_16_2),
                new MinecraftPacketMapping(0x3D, ProtocolVersion.MINECRAFT_1_17),
                new MinecraftPacketMapping(0x3B, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketMapping(0x3E, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketMapping(0x3D, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketMapping(0x41, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(RespawnPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x5F, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketMapping(0x62, ProtocolVersion.MINECRAFT_1_19_1),
                new MinecraftPacketMapping(0x60, ProtocolVersion.MINECRAFT_1_19_3),
                new MinecraftPacketMapping(0x64, ProtocolVersion.MINECRAFT_1_19_4)
            ],
            typeof(SystemChatMessagePacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        {
            [
                new MinecraftPacketMapping(0x02, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x03, ProtocolVersion.MINECRAFT_1_14, ProtocolVersion.MINECRAFT_1_18_2)
            ],
            typeof(ChatMessagePacket)
        },
        {
            [
                new MinecraftPacketMapping(0x03, ProtocolVersion.MINECRAFT_1_19),
                new MinecraftPacketMapping(0x04, ProtocolVersion.MINECRAFT_1_19_1, ProtocolVersion.MINECRAFT_1_19_1)
            ],
            typeof(KeyedChatCommandPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x04, ProtocolVersion.MINECRAFT_1_19_3)
            ],
            typeof(SignedChatCommandPacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}