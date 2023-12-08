using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct EncryptionRequestPacket : IMinecraftPacket<LoginState>
{
    public string ServerId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(ServerId ?? string.Empty);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            buffer.WriteVarInt(PublicKey.Length);
            buffer.Write(PublicKey);

            buffer.WriteVarInt(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
        else
        {
            buffer.WriteExtendedForgeShort(PublicKey.Length);
            buffer.Write(PublicKey);

            buffer.WriteExtendedForgeShort(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        ServerId = buffer.ReadString();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            var publicKeyLength = buffer.ReadVarInt();
            PublicKey = buffer.Read(publicKeyLength).ToArray();

            var verifyTokenLength = buffer.ReadVarInt();
            VerifyToken = buffer.Read(verifyTokenLength).ToArray();
        }
        else
        {
            var publicKeyLength = buffer.ReadExtendedForgeShort();
            PublicKey = buffer.Read(publicKeyLength).ToArray();

            var verifyTokenLength = buffer.ReadExtendedForgeShort();
            VerifyToken = buffer.Read(verifyTokenLength).ToArray();
        }
    }
}