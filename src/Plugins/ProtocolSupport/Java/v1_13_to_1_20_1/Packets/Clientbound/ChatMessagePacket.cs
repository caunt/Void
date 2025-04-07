using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class ChatMessagePacket : IMinecraftClientboundPacket<ChatMessagePacket>
{
    public required Component Message { get; set; }
    public byte Position { get; set; }
    public Uuid? Sender { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, protocolVersion);
        buffer.WriteUnsignedByte(Position);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            buffer.WriteUuid(Sender ?? throw new InvalidOperationException($"{nameof(Sender)} is required for this protocol version"));
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent(protocolVersion);
        var position = buffer.ReadUnsignedByte();
        var sender = protocolVersion >= ProtocolVersion.MINECRAFT_1_16 ? buffer.ReadUuid() : (Uuid?)null;

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
