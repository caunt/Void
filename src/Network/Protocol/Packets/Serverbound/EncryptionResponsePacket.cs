using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct EncryptionResponsePacket : IMinecraftPacket<LoginState>
{
    public byte[] SharedSecret { get; set; }
    public byte[] VerifyToken { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(SharedSecret.Length);
        buffer.Write(SharedSecret);
        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        SharedSecret = buffer.Read(sharedSecretLength).ToArray();

        var verifyTokenLength = buffer.ReadVarInt();
        VerifyToken = buffer.Read(verifyTokenLength).ToArray();
    }
}