using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class LoginStartPacket : IMinecraftServerboundPacket<LoginStartPacket>
{
    public required GameProfile Profile { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Profile.Username);
        buffer.WriteUuid(Profile.Id);
    }

    public static LoginStartPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var username = buffer.ReadString();
        var uuid = buffer.ReadUuid();

        return new LoginStartPacket { Profile = new GameProfile(username, uuid) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
