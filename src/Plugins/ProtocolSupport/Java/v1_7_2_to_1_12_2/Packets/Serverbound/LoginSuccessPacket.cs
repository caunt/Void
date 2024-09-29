using Void.Proxy.API.Mojang;
using Void.Proxy.API.Mojang.Profiles;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

public class LoginSuccessPacket : IMinecraftPacket<LoginSuccessPacket>
{
    public required GameProfile GameProfile { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(GameProfile.Id.ToString());
        buffer.WriteString(GameProfile.Username);
    }

    public static LoginSuccessPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginSuccessPacket { GameProfile = new GameProfile(Uuid.Parse(buffer.ReadString(36)), buffer.ReadString(), []) };
    }

    public void Dispose()
    {
    }
}