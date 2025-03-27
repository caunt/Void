using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

public class ChatMessagePacket : IMinecraftServerboundPacket<ChatMessagePacket>
{
    public required string Message { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Message);
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ChatMessagePacket
        {
            Message = buffer.ReadString()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
