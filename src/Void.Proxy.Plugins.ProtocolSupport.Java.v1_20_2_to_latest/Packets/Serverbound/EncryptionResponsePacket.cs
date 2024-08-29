using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class EncryptionResponsePacket : IMinecraftPacket<EncryptionResponsePacket>
{
    public required byte[] SharedSecret { get; set; }
    public required byte[] VerifyToken { get; set; }
    public long? Salt { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            buffer.WriteVarInt(SharedSecret.Length);
            buffer.Write(SharedSecret);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion < ProtocolVersion.MINECRAFT_1_19_3)
            {
                if (Salt.HasValue)
                {
                    buffer.WriteBoolean(false);
                    buffer.WriteLong(Salt.Value);
                }
                else
                {
                    buffer.WriteBoolean(true);
                }
            }

            buffer.WriteVarInt(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
        else
        {
            buffer.WriteVarShort(SharedSecret.Length);
            buffer.Write(SharedSecret);

            buffer.WriteVarShort(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
    }

    public static EncryptionResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        ReadOnlySpan<byte> sharedSecret;
        ReadOnlySpan<byte> verifyToken;
        long? salt = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            var sharedSecretLength = buffer.ReadVarInt();
            sharedSecret = buffer.Read(sharedSecretLength).ToArray();

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion < ProtocolVersion.MINECRAFT_1_19_3 && !buffer.ReadBoolean())
                salt = buffer.ReadLong();

            var verifyTokenLength = buffer.ReadVarInt();
            verifyToken = buffer.Read(verifyTokenLength).ToArray();
        }
        else
        {
            var sharedSecretLength = buffer.ReadVarShort();
            sharedSecret = buffer.Read(sharedSecretLength).ToArray();

            var verifyTokenLength = buffer.ReadVarShort();
            verifyToken = buffer.Read(verifyTokenLength).ToArray();
        }

        return new EncryptionResponsePacket
        {
            SharedSecret = sharedSecret.ToArray(),
            VerifyToken = verifyToken.ToArray(),
            Salt = salt
        };
    }

    public void Dispose()
    {
    }
}