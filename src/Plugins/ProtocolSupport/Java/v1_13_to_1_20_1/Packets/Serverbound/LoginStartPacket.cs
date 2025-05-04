using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class LoginStartPacket : IMinecraftServerboundPacket<LoginStartPacket>
{
    public required GameProfile Profile { get; set; }
    public required IdentifiedKey? Key { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Profile.Username);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_3)
            {
                var hasSignatureData = Key is not null;
                buffer.WriteBoolean(hasSignatureData);

                if (hasSignatureData)
                {
                    buffer.WriteLong(Key!.ExpiresAt);

                    buffer.WriteVarInt(Key.PublicKey.Length);
                    buffer.Write(Key.PublicKey);

                    buffer.WriteVarInt(Key.Signature.Length);
                    buffer.Write(Key.Signature);
                }
            }

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
            {
                if (Key is not null && Key.ProfileUuid != default)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteUuid(Key.ProfileUuid);
                }
                else if (Profile.Id.AsGuid != default)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteUuid(Profile.Id);
                }
                else
                {
                    buffer.WriteBoolean(false);
                }
            }
        }
    }

    public static LoginStartPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var username = buffer.ReadString();
        var uuid = Uuid.Empty;
        IdentifiedKey? key = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (protocolVersion <= ProtocolVersion.MINECRAFT_1_19_1)
            {
                var hasSignatureData = buffer.ReadBoolean();

                if (hasSignatureData)
                {
                    var revision = protocolVersion == ProtocolVersion.MINECRAFT_1_19 ? IdentifiedKeyRevision.GenericV1Revision : IdentifiedKeyRevision.LinkedV2Revision;
                    var expiresAt = buffer.ReadLong();

                    var publicKeyLength = buffer.ReadVarInt();
                    var publicKey = buffer.Read(publicKeyLength).ToArray();

                    var signatureLength = buffer.ReadVarInt();
                    var signature = buffer.Read(signatureLength).ToArray();

                    key = new IdentifiedKey(revision, expiresAt, publicKey, signature);

                    if (key.ProfileUuid != default)
                        uuid = key.ProfileUuid;
                }
            }

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
            {
                var hasUuid = buffer.ReadBoolean();

                if (hasUuid)
                    uuid = buffer.ReadUuid();
            }
        }

        return new LoginStartPacket
        {
            Profile = new GameProfile(username, uuid),
            Key = key
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
