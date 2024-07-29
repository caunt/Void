using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

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
            buffer.WriteVarShort(PublicKey.Length);
            buffer.Write(PublicKey);

            buffer.WriteVarShort(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
    }

    public async Task<bool> HandleAsync(LoginState state)
    {
        return await state.HandleAsync(this);
    }

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
            var publicKeyLength = buffer.ReadVarShort();
            PublicKey = buffer.Read(publicKeyLength).ToArray();

            var verifyTokenLength = buffer.ReadVarShort();
            VerifyToken = buffer.Read(verifyTokenLength).ToArray();
        }
    }
}