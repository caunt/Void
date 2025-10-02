using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

public class LoginPluginRequestPacket : IMinecraftClientboundPacket<LoginPluginRequestPacket>
{
    public required int MessageId { get; set; }
    public required string Channel { get; set; }
    public required byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteString(Channel);
        buffer.Write(Data);
    }

    public static LoginPluginRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginPluginRequestPacket
        {
            MessageId = buffer.ReadVarInt(),
            Channel = buffer.ReadString(),
            Data = buffer.ReadToEnd().ToArray()
        };
    }

    public static void Register(IPlayer player)
    {
        player.RegisterPacket<LoginPluginRequestPacket>([new(0x04, ProtocolVersion.Oldest)]);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
