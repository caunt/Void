using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct EncryptionResponsePacket : IMinecraftPacket<LoginState>
{
    public byte[] SharedSecret { get; set; }
    public byte[] VerifyToken { get; set; }
    public long? Salt { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            buffer.WriteVarInt(SharedSecret.Length);
            buffer.Write(SharedSecret);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion < ProtocolVersion.MINECRAFT_1_19_3)
            {
                if (Salt.HasValue)
                {
                    buffer.WriteBoolean(false);
                    buffer.WriteLong(Salt.Value);
                }
                else
                {
                    buffer.WriteBoolean(true);
                }
            }

            buffer.WriteVarInt(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
        else
        {
            buffer.WriteExtendedForgeShort(SharedSecret.Length);
            buffer.Write(SharedSecret);

            buffer.WriteExtendedForgeShort(VerifyToken.Length);
            buffer.Write(VerifyToken);
        }
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            var sharedSecretLength = buffer.ReadVarInt();
            SharedSecret = buffer.Read(sharedSecretLength).ToArray();

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && protocolVersion < ProtocolVersion.MINECRAFT_1_19_3 && !buffer.ReadBoolean())
                Salt = buffer.ReadLong();

            var verifyTokenLength = buffer.ReadVarInt();
            VerifyToken = buffer.Read(verifyTokenLength).ToArray();
        }
        else
        {
            var sharedSecretLength = buffer.ReadExtendedForgeShort();
            SharedSecret = buffer.Read(sharedSecretLength).ToArray();

            var verifyTokenLength = buffer.ReadExtendedForgeShort();
            VerifyToken = buffer.Read(verifyTokenLength).ToArray();
        }
    }
}