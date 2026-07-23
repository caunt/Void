using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class LoginSuccessPacket : IMinecraftClientboundPacket<LoginSuccessPacket>
{
    public required GameProfile GameProfile { get; set; }
    public required bool? StrictErrorHandling { get; set; }
    public Uuid? SessionId { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUuid(GameProfile.Id);
        buffer.WriteString(GameProfile.Username);
        buffer.WritePropertyArray(GameProfile.Properties);

        if (protocolVersion < ProtocolVersion.MINECRAFT_1_20_5 || protocolVersion > ProtocolVersion.MINECRAFT_1_21)
        {
            if (protocolVersion < ProtocolVersion.MINECRAFT_26_2)
                return;
        }
        else
        {
            if (!StrictErrorHandling.HasValue)
                throw new InvalidDataException(nameof(StrictErrorHandling));

            buffer.WriteBoolean(StrictErrorHandling.Value);
        }

        if (protocolVersion < ProtocolVersion.MINECRAFT_26_2)
            return;

        if (!SessionId.HasValue)
            throw new InvalidDataException(nameof(SessionId));

        buffer.WriteUuid(SessionId.Value);
    }

    public static LoginSuccessPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var uuid = buffer.ReadUuid();
        var username = buffer.ReadString();
        var properties = buffer.ReadPropertyArray();
        bool? strictErrorHandling = null;
        Uuid? sessionId = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5 && protocolVersion <= ProtocolVersion.MINECRAFT_1_21)
            strictErrorHandling = buffer.ReadBoolean();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_26_2)
            sessionId = buffer.ReadUuid();

        return new LoginSuccessPacket
        {
            GameProfile = new GameProfile(username, uuid, properties),
            StrictErrorHandling = strictErrorHandling,
            SessionId = sessionId
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
