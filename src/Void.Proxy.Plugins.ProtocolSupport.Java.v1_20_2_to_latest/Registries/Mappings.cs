using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public static class Mappings
{
    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ClientboundHandshakeMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ServerboundHandshakeMappings = new Dictionary<PacketMapping[], PacketFactory> { { [new PacketMapping(0x00, false, ProtocolVersion.MINECRAFT_1_7_2)], () => new HandshakePacket() } };

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ClientboundLoginMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ServerboundLoginMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ClientboundConfigurationMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ServerboundConfigurationMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ClientboundPlayMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static readonly IReadOnlyDictionary<PacketMapping[], PacketFactory> ServerboundPlayMappings = new Dictionary<PacketMapping[], PacketFactory>();

    public static void Fill()
    {
    } // will initialize static fields
}