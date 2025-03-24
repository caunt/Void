using Void.Proxy.Api.Mojang;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Mojang.Profiles;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

public class LoginStartPacket : IMinecraftServerboundPacket<LoginStartPacket>
{
    public required GameProfile Profile { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Profile.Username);
    }

    public static LoginStartPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginStartPacket { Profile = new GameProfile(Uuid.Empty, buffer.ReadString(), []) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}