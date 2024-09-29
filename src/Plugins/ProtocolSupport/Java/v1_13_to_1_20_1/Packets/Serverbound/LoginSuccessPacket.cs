using Void.Proxy.API.Mojang;
using Void.Proxy.API.Mojang.Profiles;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class LoginSuccessPacket : IMinecraftPacket<LoginSuccessPacket>
{
    public required GameProfile GameProfile { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
            buffer.WriteUuid(GameProfile.Id);
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            buffer.WriteUuidAsIntArray(GameProfile.Id);
        else
            buffer.WriteString(GameProfile.Id.ToString());

        buffer.WriteString(GameProfile.Username);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
            buffer.WritePropertyArray(GameProfile.Properties);
    }

    public static LoginSuccessPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var uuid = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_19 => buffer.ReadUuid(),
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_16 => buffer.ReadUuidAsIntArray(),
            _ => Uuid.Parse(buffer.ReadString(36))
        };

        var username = buffer.ReadString();

        var properties = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_19 => buffer.ReadPropertyArray(),
            _ => []
        };

        return new LoginSuccessPacket { GameProfile = new GameProfile(uuid, username, properties) };
    }

    public void Dispose()
    {
    }
}