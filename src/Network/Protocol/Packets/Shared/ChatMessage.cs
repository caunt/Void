using MinecraftProxy.Models.Minecraft.Chat;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Shared;

public interface IChatMessage
{
    public string Message { get; set; }
}

public interface IChatCommand
{
    public string Command { get; set; }
}

public struct ChatMessage : IMinecraftPacket<PlayState>, IChatMessage, IChatCommand
{
    public string Command { get => Message.TrimStart('/'); set => Message = "/" + value; }
    public string Message { get; set; }
    public ChatMessageType Type { get; set; }
    public Guid? Sender { get; set; }
    public Direction Direction { get; }

    public ChatMessage() : this(0) { throw new NotSupportedException("Specify direction to chat message"); }

    public ChatMessage(Direction direction) => Direction = direction;

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        ArgumentNullException.ThrowIfNull(Message);
        buffer.WriteString(Message);

        if (Direction == Direction.Clientbound && protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            buffer.WriteUnsignedByte((byte)Type);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
                buffer.WriteGuid(Sender ?? Guid.Empty);
        }
    }

    public async Task<bool> HandleAsync(PlayState state) => await (Message.StartsWith('/') ? state.HandleAsync(this as IChatCommand) : state.HandleAsync(this as IChatMessage));

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Message = buffer.ReadString(256);

        if (Direction == Direction.Clientbound && protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            Type = (ChatMessageType)buffer.ReadUnsignedByte();

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
                Sender = buffer.ReadGuid();
        }
    }
}