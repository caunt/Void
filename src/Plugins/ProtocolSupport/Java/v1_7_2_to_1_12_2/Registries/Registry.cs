using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

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
        { [new MinecraftPacketMapping(0x03, ProtocolVersion.MINECRAFT_1_8)], typeof(SetCompressionPacket) },
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
        {
            [
                new MinecraftPacketMapping(0x40, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_9)
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new MinecraftPacketMapping(0x01, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x23, ProtocolVersion.MINECRAFT_1_9)
            ],
            typeof(JoinGamePacket)
        },
        {
            [
                new MinecraftPacketMapping(0x07, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x33, ProtocolVersion.MINECRAFT_1_9),
                new MinecraftPacketMapping(0x34, ProtocolVersion.MINECRAFT_1_12),
                new MinecraftPacketMapping(0x35, ProtocolVersion.MINECRAFT_1_12_1)
            ],
            typeof(RespawnPacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketMapping[], Type>
    {
        {
            [
                new MinecraftPacketMapping(0x01, Plugin.SupportedVersions.First()),
                new MinecraftPacketMapping(0x02, ProtocolVersion.MINECRAFT_1_9),
                new MinecraftPacketMapping(0x03, ProtocolVersion.MINECRAFT_1_12),
                new MinecraftPacketMapping(0x02, ProtocolVersion.MINECRAFT_1_12_1)
            ],
            typeof(ChatMessagePacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}