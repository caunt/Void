using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class ChatMessagePacket : IMinecraftClientboundPacket<ChatMessagePacket>
{
    public required Component Message { get; set; }
    public byte? Position { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, protocolVersion);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteUnsignedByte(Position ?? throw new InvalidOperationException($"{nameof(Position)} is required for this protocol version"));
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent(protocolVersion);
        var position = protocolVersion >= ProtocolVersion.MINECRAFT_1_8 ? buffer.ReadUnsignedByte() : (byte?)null;

        return new ChatMessagePacket
        {
            Message = message,
            Position = position
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
