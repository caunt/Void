using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

public class EncryptionResponsePacket : IMinecraftServerboundPacket<EncryptionResponsePacket>
{
    public required byte[] SharedSecret { get; set; }
    public required byte[] VerifyToken { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteVarInt(SharedSecret.Length);
        else
            buffer.WriteUnsignedShort((ushort)SharedSecret.Length);

        buffer.Write(SharedSecret);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteVarInt(VerifyToken.Length);
        else
            buffer.WriteUnsignedShort((ushort)VerifyToken.Length);

        buffer.Write(VerifyToken);
    }

    public static EncryptionResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var sharedSecretLength = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_8 => buffer.ReadVarInt(),
            _ => buffer.ReadUnsignedShort()
        };

        var sharedSecret = buffer.Read(sharedSecretLength);

        var verifyTokenLength = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_8 => buffer.ReadVarInt(),
            _ => buffer.ReadUnsignedShort()
        };

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