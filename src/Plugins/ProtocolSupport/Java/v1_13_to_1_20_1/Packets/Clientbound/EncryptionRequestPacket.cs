using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class EncryptionRequestPacket : IMinecraftClientboundPacket<EncryptionRequestPacket>
{
    public required string ServerId { get; set; }
    public required byte[] PublicKey { get; set; }
    public required byte[] VerifyToken { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(ServerId);

        buffer.WriteVarInt(PublicKey.Length);
        buffer.Write(PublicKey);

        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public static EncryptionRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var serverId = buffer.ReadString();

        var publicKeyLength = buffer.ReadVarInt();
        var publicKey = buffer.Read(publicKeyLength);

        var verifyTokenLength = buffer.ReadVarInt();
        var verifyToken = buffer.Read(verifyTokenLength);

        return new EncryptionRequestPacket
        {
            ServerId = serverId,
            PublicKey = publicKey.ToArray(),
            VerifyToken = verifyToken.ToArray()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}