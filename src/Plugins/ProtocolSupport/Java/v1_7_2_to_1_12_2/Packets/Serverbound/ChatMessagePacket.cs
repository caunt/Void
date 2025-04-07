using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

public class ChatMessagePacket : IMinecraftServerboundPacket<ChatMessagePacket>
{
    public required Component Message { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Message.SerializeLegacy());
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ChatMessagePacket { Message = buffer.ReadComponent(protocolVersion) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
