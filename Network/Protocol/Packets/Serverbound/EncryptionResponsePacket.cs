using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public class EncryptionResponsePacket : IMinecraftPacket<LoginState>
{
    public byte[] SharedSecret { get; set; }
    public byte[] VerifyToken { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(SharedSecret.Length);
        buffer.Write(SharedSecret);
        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        SharedSecret = buffer.ReadUInt8Array(sharedSecretLength);

        var verifyTokenLength = buffer.ReadVarInt();
        VerifyToken = buffer.ReadUInt8Array(verifyTokenLength);
    }
}