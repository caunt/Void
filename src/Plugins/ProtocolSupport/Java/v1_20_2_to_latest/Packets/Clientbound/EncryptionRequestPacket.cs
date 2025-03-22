using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class EncryptionRequestPacket : IMinecraftClientboundPacket<EncryptionRequestPacket>
{
    public required string ServerId { get; set; }
    public required byte[] PublicKey { get; set; }
    public required byte[] VerifyToken { get; set; }
    public bool? ShouldAuthenticate { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(ServerId);

        buffer.WriteVarInt(PublicKey.Length);
        buffer.Write(PublicKey);

        buffer.WriteVarInt(VerifyToken.Length);
        buffer.Write(VerifyToken);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
        {
            if (!ShouldAuthenticate.HasValue)
                throw new InvalidDataException($"{nameof(ShouldAuthenticate)} is not set");

            buffer.WriteBoolean(ShouldAuthenticate.Value);
        }
    }

    public static EncryptionRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var serverId = buffer.ReadString();

        var publicKeyLength = buffer.ReadVarInt();
        var publicKey = buffer.Read(publicKeyLength);

        var verifyTokenLength = buffer.ReadVarInt();
        var verifyToken = buffer.Read(verifyTokenLength);

        bool? shouldAuthenticate = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            shouldAuthenticate = buffer.ReadBoolean();

        return new EncryptionRequestPacket
        {
            ServerId = serverId,
            PublicKey = publicKey.ToArray(),
            VerifyToken = verifyToken.ToArray(),
            ShouldAuthenticate = shouldAuthenticate
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}