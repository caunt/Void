﻿using Void.Proxy.API.Network.Protocol;
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
        // { [new PacketMapping(0x00, ProtocolVersion.MINECRAFT_1_7_2)], typeof(DisconnectPacket) },
        // { [new PacketMapping(0x01, ProtocolVersion.MINECRAFT_1_7_2)], typeof(EncryptionRequestPacket) },
        // { [new PacketMapping(0x02, ProtocolVersion.MINECRAFT_1_7_2)], typeof(LoginSuccessPacket) },
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_8)], typeof(SetCompressionPacket) },
        // { [new PacketMapping(0x04, ProtocolVersion.MINECRAFT_1_13)], typeof(LoginPluginRequest) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        // { [new PacketMapping(0x00, ProtocolVersion.MINECRAFT_1_7_2)], typeof(LoginStartPacket) },
        // { [new PacketMapping(0x01, ProtocolVersion.MINECRAFT_1_7_2)], typeof(EncryptionResponsePacket) },
        // { [new PacketMapping(0x02, ProtocolVersion.MINECRAFT_1_13)], typeof(LoginPluginResponse) },
        // { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_20_2)], typeof(LoginAcknowledgedPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundConfigurationMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundConfigurationMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundPlayMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>();

    public static void Fill()
    {
        // will initialize static fields
    }
}