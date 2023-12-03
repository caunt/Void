using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public class EncryptionRequestPacket : IMinecraftPacket<LoginState>
{
    public string ServerId { get; set; } = string.Empty; // must be empty string by default
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteString(ServerId);
        buffer.WriteVarInt(PublicKey.Length);
        buffer.Write(PublicKey);
        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        ServerId = buffer.ReadString();

        var publicKeyLength = buffer.ReadVarInt();
        PublicKey = buffer.ReadUInt8Array(publicKeyLength);

        var verifyTokenLength = buffer.ReadVarInt();
        VerifyToken = buffer.ReadUInt8Array(verifyTokenLength);
    }
}