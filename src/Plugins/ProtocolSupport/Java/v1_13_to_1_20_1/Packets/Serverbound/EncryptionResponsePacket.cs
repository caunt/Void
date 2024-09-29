using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class EncryptionResponsePacket : IMinecraftPacket<EncryptionResponsePacket>
{
    public required byte[] SharedSecret { get; set; }
    public required byte[] VerifyToken { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(SharedSecret.Length);
        buffer.Write(SharedSecret);
        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public static EncryptionResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        var sharedSecret = buffer.Read(sharedSecretLength);
        var verifyTokenLength = buffer.ReadVarInt();
        var verifyToken = buffer.Read(verifyTokenLength);

        return new EncryptionResponsePacket
        {
            SharedSecret = sharedSecret.ToArray(),
            VerifyToken = verifyToken.ToArray()
        };
    }

    public void Dispose()
    {
    }
}