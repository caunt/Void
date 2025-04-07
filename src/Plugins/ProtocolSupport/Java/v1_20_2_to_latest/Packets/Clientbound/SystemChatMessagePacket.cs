using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    public required Component Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, protocolVersion);
        buffer.WriteBoolean(Overlay);
    }

    public static SystemChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent(protocolVersion);
        var overlay = buffer.ReadBoolean();

        return new SystemChatMessagePacket
        {
            Message = message,
            Overlay = overlay
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
