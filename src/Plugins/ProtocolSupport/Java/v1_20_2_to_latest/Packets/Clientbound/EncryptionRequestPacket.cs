using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class EncryptionRequestPacket : IMinecraftPacket<EncryptionRequestPacket>
{
    public required string ServerId { get; set; }
    public required byte[] PublicKey { get; set; }
    public required byte[] VerifyToken { get; set; }
    public bool? ShouldAuthenticate { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(ServerId);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            buffer.WriteVarInt(PublicKey.Length);
            buffer.Write(PublicKey);

            buffer.WriteVarInt(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
        else
        {
            buffer.WriteVarShort(PublicKey.Length);
            buffer.Write(PublicKey);

            buffer.WriteVarShort(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
        {
            if (!ShouldAuthenticate.HasValue)
                throw new InvalidDataException($"{nameof(ShouldAuthenticate)} is not set");

            buffer.WriteBoolean(ShouldAuthenticate.Value);
        }
    }

    public static EncryptionRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var serverId = buffer.ReadString();

        ReadOnlySpan<byte> publicKey;
        ReadOnlySpan<byte> verifyToken;
        bool? shouldAuthenticate = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            var publicKeyLength = buffer.ReadVarInt();
            publicKey = buffer.Read(publicKeyLength);

            var verifyTokenLength = buffer.ReadVarInt();
            verifyToken = buffer.Read(verifyTokenLength);
        }
        else
        {
            var publicKeyLength = buffer.ReadVarShort();
            publicKey = buffer.Read(publicKeyLength);

            var verifyTokenLength = buffer.ReadVarShort();
            verifyToken = buffer.Read(verifyTokenLength);
        }

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            shouldAuthenticate = buffer.ReadBoolean();

        return new EncryptionRequestPacket
        {
            ServerId = serverId,
            PublicKey = publicKey.ToArray(),
            VerifyToken = verifyToken.ToArray(),
            ShouldAuthenticate = shouldAuthenticate
        };
    }

    public void Dispose()
    {
    }
}