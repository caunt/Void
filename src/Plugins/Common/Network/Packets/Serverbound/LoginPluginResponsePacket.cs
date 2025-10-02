using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Network.Packets.Serverbound;

public class LoginPluginResponsePacket : IMinecraftServerboundPacket<LoginPluginResponsePacket>
{
    public required int MessageId { get; set; }
    public required bool Successful { get; set; }
    public required byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBoolean(Successful);
        buffer.Write(Data);
    }

    public static LoginPluginResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginPluginResponsePacket
        {
            MessageId = buffer.ReadVarInt(),
            Successful = buffer.ReadBoolean(),
            Data = buffer.ReadToEnd().ToArray()
        };
    }

    public static void Register(IPlayer player)
    {
        player.RegisterPacket<LoginPluginResponsePacket>([new(0x04, ProtocolVersion.Oldest)]);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
