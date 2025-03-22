using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class EncryptionResponsePacket : IServerboundPacket<EncryptionResponsePacket>
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
        GC.SuppressFinalize(this);
    }
}