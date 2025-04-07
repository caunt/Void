using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

public class ChatMessagePacket : IMinecraftServerboundPacket<ChatMessagePacket>
{
    public required string Text { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Text);
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ChatMessagePacket { Text = buffer.ReadString() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
