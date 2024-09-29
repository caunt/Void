using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class EncryptionRequestPacket : IMinecraftPacket<EncryptionRequestPacket>
{
    public required string ServerId { get; set; }
    public required byte[] PublicKey { get; set; }
    public required byte[] VerifyToken { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(ServerId);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteVarInt(PublicKey.Length);
        else
            buffer.WriteUnsignedShort((ushort)PublicKey.Length);

        buffer.Write(PublicKey);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteVarInt(VerifyToken.Length);
        else
            buffer.WriteUnsignedShort((ushort)VerifyToken.Length);

        buffer.Write(VerifyToken);
    }

    public static EncryptionRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var serverId = buffer.ReadString();

        var publicKeyLength = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_8 => buffer.ReadVarInt(),
            _ => buffer.ReadUnsignedShort()
        };

        var publicKey = buffer.Read(publicKeyLength);

        var verifyTokenLength = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_8 => buffer.ReadVarInt(),
            _ => buffer.ReadUnsignedShort()
        };

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
    }
}