using Void.Proxy.API.Mojang.Profiles;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class LoginSuccessPacket : IMinecraftPacket<LoginSuccessPacket>
{
    public required GameProfile GameProfile { get; set; }
    public required bool? StrictErrorHandling { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUuid(GameProfile.Id);
        buffer.WriteString(GameProfile.Username);
        buffer.WritePropertyArray(GameProfile.Properties);

        if (protocolVersion < ProtocolVersion.MINECRAFT_1_20_5)
            return;

        if (!StrictErrorHandling.HasValue)
            throw new ArgumentNullException(nameof(StrictErrorHandling));

        buffer.WriteBoolean(StrictErrorHandling.Value);
    }

    public static LoginSuccessPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var uuid = buffer.ReadUuid();
        var username = buffer.ReadString();
        var properties = buffer.ReadPropertyArray();
        bool? strictErrorHandling = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            strictErrorHandling = buffer.ReadBoolean();

        return new LoginSuccessPacket
        {
            GameProfile = new GameProfile(uuid, username, properties),
            StrictErrorHandling = strictErrorHandling
        };
    }

    public void Dispose()
    {
    }
}