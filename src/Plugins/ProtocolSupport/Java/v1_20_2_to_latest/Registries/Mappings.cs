using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.Protocol;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public static class Mappings
{
    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundHandshakeMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundHandshakeMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionRequestPacket) },
        { [new PacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginSuccessPacket) },
        { [new PacketMapping(0x03, Plugin.SupportedVersions.First())], typeof(SetCompressionPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginStartPacket) },
        { [new PacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionResponsePacket) },
        { [new PacketMapping(0x03, Plugin.SupportedVersions.First())], typeof(LoginAcknowledgedPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundConfigurationMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x02, Plugin.SupportedVersions.First(), ProtocolVersion.MINECRAFT_1_20_3)], typeof(FinishConfigurationPacket) },
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_20_5)], typeof(FinishConfigurationPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundConfigurationMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x24, Plugin.SupportedVersions.First(), ProtocolVersion.MINECRAFT_1_20_3)], typeof(KeepAliveRequestPacket) },
        { [new PacketMapping(0x26, ProtocolVersion.MINECRAFT_1_20_5)], typeof(KeepAliveRequestPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x14, Plugin.SupportedVersions.First(), ProtocolVersion.MINECRAFT_1_20_2)], typeof(KeepAliveResponsePacket) },
        { [new PacketMapping(0x15, ProtocolVersion.MINECRAFT_1_20_3, ProtocolVersion.MINECRAFT_1_20_3)], typeof(KeepAliveResponsePacket) },
        { [new PacketMapping(0x18, ProtocolVersion.MINECRAFT_1_20_5)], typeof(KeepAliveResponsePacket) }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}