using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class EncryptionResponsePacket : IMinecraftPacket<EncryptionResponsePacket>
{
    public required byte[] SharedSecret { get; set; }
    public required byte[]? VerifyToken { get; set; }

    // 1.19 - 1.19.2
    public required long? Salt { get; set; }
    public required byte[]? MessageSignature { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(SharedSecret.Length);
        buffer.Write(SharedSecret);

        var hasVerifyToken = VerifyToken is not null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion < ProtocolVersion.MINECRAFT_1_19_3 && !hasVerifyToken)
        {
            buffer.WriteBoolean(hasVerifyToken);

            if (Salt is null)
                throw new ArgumentNullException(nameof(Salt));

            if (MessageSignature is null)
                throw new ArgumentNullException(nameof(MessageSignature));

            buffer.WriteLong(Salt.Value);
            buffer.WriteVarInt(MessageSignature.Length);
            buffer.Write(MessageSignature);
        }

        if (hasVerifyToken)
        {
            buffer.WriteVarInt(VerifyToken!.Length);
            buffer.Write(VerifyToken);
        }
    }

    public static EncryptionResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        var sharedSecret = buffer.Read(sharedSecretLength).ToArray();

        var hasVerifyToken = true;
        byte[]? verifyToken = null;
        long? salt = null;
        byte[]? messageSignature = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion < ProtocolVersion.MINECRAFT_1_19_3)
        {
            hasVerifyToken = buffer.ReadBoolean();

            if (!hasVerifyToken)
            {
                salt = buffer.ReadLong();

                var messageSignatureLength = buffer.ReadVarInt();
                messageSignature = buffer.Read(messageSignatureLength).ToArray();
            }
        }

        if (hasVerifyToken)
        {
            var verifyTokenLength = buffer.ReadVarInt();
            verifyToken = buffer.Read(verifyTokenLength).ToArray();
        }

        return new EncryptionResponsePacket
        {
            SharedSecret = sharedSecret,
            VerifyToken = verifyToken,
            Salt = salt,
            MessageSignature = messageSignature
        };
    }

    public void Dispose()
    {
    }
}