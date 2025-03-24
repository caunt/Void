using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}