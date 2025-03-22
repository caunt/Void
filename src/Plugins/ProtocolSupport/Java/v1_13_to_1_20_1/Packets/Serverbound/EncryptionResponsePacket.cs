using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class EncryptionResponsePacket : IServerboundPacket<EncryptionResponsePacket>
{
    public required byte[] SharedSecret { get; set; }
    public required byte[] VerifyToken { get; set; }

    // 1.19 - 1.19.2
    public required long Salt { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(SharedSecret.Length);
        buffer.Write(SharedSecret);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion <= ProtocolVersion.MINECRAFT_1_19_1)
        {
            var hasVerifyToken = Salt is default(long);
            buffer.WriteBoolean(hasVerifyToken);

            if (!hasVerifyToken)
                buffer.WriteLong(Salt);
        }

        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public static EncryptionResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        var sharedSecret = buffer.Read(sharedSecretLength).ToArray();

        var salt = default(long);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion <= ProtocolVersion.MINECRAFT_1_19_1)
        {
            var hasVerifyToken = buffer.ReadBoolean();

            if (!hasVerifyToken)
                salt = buffer.ReadLong();
        }

        var verifyTokenLength = buffer.ReadVarInt();
        var verifyToken = buffer.Read(verifyTokenLength).ToArray();

        return new EncryptionResponsePacket
        {
            SharedSecret = sharedSecret,
            VerifyToken = verifyToken,
            Salt = salt
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}