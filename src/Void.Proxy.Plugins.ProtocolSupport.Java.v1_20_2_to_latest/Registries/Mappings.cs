using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public static class Mappings
{
    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundHandshakeMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundHandshakeMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, ProtocolVersion.MINECRAFT_1_7_2)], typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_8)], typeof(SetCompressionPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_20_2)], typeof(LoginAcknowledgedPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundConfigurationMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_21)], typeof(FinishConfigurationPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundConfigurationMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x26, ProtocolVersion.MINECRAFT_1_21)], typeof(KeepAliveRequestPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x18, ProtocolVersion.MINECRAFT_1_21)], typeof(KeepAliveResponsePacket) }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}