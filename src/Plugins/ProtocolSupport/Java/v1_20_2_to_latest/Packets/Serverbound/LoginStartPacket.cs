using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Mojang.Profiles;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class LoginStartPacket : IServerboundPacket<LoginStartPacket>
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

        return new LoginStartPacket { Profile = new GameProfile(uuid, username, []) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}