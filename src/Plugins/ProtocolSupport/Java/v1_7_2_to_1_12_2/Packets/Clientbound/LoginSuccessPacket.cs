using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class LoginSuccessPacket : IMinecraftClientboundPacket<LoginSuccessPacket>
{
    public required GameProfile GameProfile { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var id = protocolVersion switch
        {
            _ when protocolVersion >= ProtocolVersion.MINECRAFT_1_7_6 => GameProfile.Id.ToString(),
            _ => GameProfile.Id.ToString().Replace("-", string.Empty)
        };

        buffer.WriteString(id);
        buffer.WriteString(GameProfile.Username);
    }

    public static LoginSuccessPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var uuid = Uuid.Parse(buffer.ReadString(36));
        var name = buffer.ReadString();

        return new LoginSuccessPacket { GameProfile = new GameProfile(name, uuid) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
