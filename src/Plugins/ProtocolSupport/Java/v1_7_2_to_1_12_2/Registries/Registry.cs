using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

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
        { [new MinecraftPacketIdMapping(0x03, ProtocolVersion.MINECRAFT_1_8)], typeof(SetCompressionPacket) },
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
        {
            [
                new MinecraftPacketIdMapping(0x40, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x1A, ProtocolVersion.MINECRAFT_1_9)
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x01, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x23, ProtocolVersion.MINECRAFT_1_9)
            ],
            typeof(JoinGamePacket)
        },
        {
            [
                new MinecraftPacketIdMapping(0x07, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x33, ProtocolVersion.MINECRAFT_1_9),
                new MinecraftPacketIdMapping(0x34, ProtocolVersion.MINECRAFT_1_12),
                new MinecraftPacketIdMapping(0x35, ProtocolVersion.MINECRAFT_1_12_1)
            ],
            typeof(RespawnPacket)
        }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        {
            [
                new MinecraftPacketIdMapping(0x01, Plugin.SupportedVersions.First()),
                new MinecraftPacketIdMapping(0x02, ProtocolVersion.MINECRAFT_1_9),
                new MinecraftPacketIdMapping(0x03, ProtocolVersion.MINECRAFT_1_12),
                new MinecraftPacketIdMapping(0x02, ProtocolVersion.MINECRAFT_1_12_1)
            ],
            typeof(ChatMessagePacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}
