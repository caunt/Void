using Minecraft.Component.Component;
using Void.Proxy.Models.Minecraft.Chat;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct SystemChatMessage : IMinecraftPacket<PlayState>
{
    public ChatComponent Message { get; set; }
    public ChatMessageType Type { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, protocolVersion);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            switch (Type)
            {
                case ChatMessageType.System:
                    buffer.WriteBoolean(false);
                    break;
                case ChatMessageType.GameInfo:
                    buffer.WriteBoolean(true);
                    break;
                default:
                    throw new NotSupportedException($"Chat message type {Type} not supported from 1.19.1");
            }
        }
        else
        {
            buffer.WriteVarInt((int)Type);
        }
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Message = buffer.ReadComponent(262144, protocolVersion);
        Type = (ChatMessageType)buffer.ReadVarInt();
    }
}
