using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

public class ChatMessagePacket : IMinecraftClientboundPacket<ChatMessagePacket>
{
    public required Component Message { get; set; }
    public byte Position { get; set; }
    public Uuid Sender { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, asNbt: false);
        buffer.WriteUnsignedByte(Position);
        buffer.WriteUuid(Sender);
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent(asNbt: false);
        var position = buffer.ReadUnsignedByte();
        var sender = buffer.ReadUuid();

        return new ChatMessagePacket
        {
            Message = message,
            Position = position,
            Sender = sender
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
