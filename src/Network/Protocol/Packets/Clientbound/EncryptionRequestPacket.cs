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
        buffer.WriteVarInt(PublicKey.Length);
        buffer.Write(PublicKey);
        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        ServerId = buffer.ReadString();

        var publicKeyLength = buffer.ReadVarInt();
        PublicKey = buffer.Read(publicKeyLength).ToArray();

        var verifyTokenLength = buffer.ReadVarInt();
        VerifyToken = buffer.Read(verifyTokenLength).ToArray();
    }
}