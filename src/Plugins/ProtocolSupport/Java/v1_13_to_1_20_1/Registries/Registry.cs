using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

public static class Registry
{
    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundHandshakeMappings = new Dictionary<MinecraftPacketIdMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundHandshakeMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { PacketIdDefinitions.ServerboundHandshake, typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundStatusMappings = new Dictionary<MinecraftPacketIdMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundStatusMappings = new Dictionary<MinecraftPacketIdMapping[], Type>();

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundLoginMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { PacketIdDefinitions.ClientboundLoginDisconnect, typeof(JsonDisconnectPacket) },
        { PacketIdDefinitions.ClientboundEncryptionRequest, typeof(EncryptionRequestPacket) },
        { PacketIdDefinitions.ClientboundLoginSuccess, typeof(LoginSuccessPacket) },
        { PacketIdDefinitions.ClientboundSetCompression, typeof(SetCompressionPacket) },
        { PacketIdDefinitions.ClientboundLoginPluginRequest, typeof(LoginPluginRequestPacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundLoginMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { PacketIdDefinitions.ServerboundLoginStart, typeof(LoginStartPacket) },
        { PacketIdDefinitions.ServerboundEncryptionResponse, typeof(EncryptionResponsePacket) },
        { PacketIdDefinitions.ServerboundLoginPluginResponse, typeof(LoginPluginResponsePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ClientboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { PacketIdDefinitions.ClientboundBundleDelimiter, typeof(BundleDelimiterPacket) },
        { PacketIdDefinitions.ClientboundPlayDisconnect, typeof(JsonDisconnectPacket) },
        { PacketIdDefinitions.ClientboundPlayKeepAliveRequest, typeof(KeepAliveRequestPacket) },
        { PacketIdDefinitions.ClientboundJoinGame, typeof(JoinGamePacket) },
        { PacketIdDefinitions.ClientboundRespawn, typeof(RespawnPacket) },
        { PacketIdDefinitions.ClientboundSystemChatMessage, typeof(SystemChatMessagePacket) },
        { PacketIdDefinitions.ClientboundChatMessage, typeof(Common.Network.Packets.Clientbound.ChatMessagePacket) }
    };

    public static readonly IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> ServerboundPlayMappings = new Dictionary<MinecraftPacketIdMapping[], Type>
    {
        { PacketIdDefinitions.ServerboundPlayKeepAliveResponse, typeof(KeepAliveResponsePacket) },
        { PacketIdDefinitions.ServerboundChatMessage, typeof(Packets.Serverbound.ChatMessagePacket) },
        { PacketIdDefinitions.ServerboundKeyedChatCommand, typeof(KeyedChatCommandPacket) },
        { PacketIdDefinitions.ServerboundSignedChatCommand, typeof(SignedChatCommandPacket) }
    };

    public static void Fill()
    {
        // will initialize static fields
    }
}
