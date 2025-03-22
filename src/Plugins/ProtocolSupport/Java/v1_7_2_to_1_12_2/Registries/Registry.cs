using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

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
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_8)], typeof(SetCompressionPacket) },
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
        {
            [
                new PacketMapping(0x40, Plugin.SupportedVersions.First()),
                new PacketMapping(0x1A, ProtocolVersion.MINECRAFT_1_9)
            ],
            typeof(PlayDisconnectPacket)
        },
        {
            [
                new PacketMapping(0x01, Plugin.SupportedVersions.First()),
                new PacketMapping(0x23, ProtocolVersion.MINECRAFT_1_9)
            ],
            typeof(JoinGamePacket)
        },
        {
            [
                new PacketMapping(0x07, Plugin.SupportedVersions.First()),
                new PacketMapping(0x33, ProtocolVersion.MINECRAFT_1_9),
                new PacketMapping(0x34, ProtocolVersion.MINECRAFT_1_12),
                new PacketMapping(0x35, ProtocolVersion.MINECRAFT_1_12_1)
            ],
            typeof(RespawnPacket)
        }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        {
            [
                new PacketMapping(0x01, Plugin.SupportedVersions.First()),
                new PacketMapping(0x02, ProtocolVersion.MINECRAFT_1_9),
                new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_12),
                new PacketMapping(0x02, ProtocolVersion.MINECRAFT_1_12_1)
            ],
            typeof(ChatMessagePacket)
        }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}