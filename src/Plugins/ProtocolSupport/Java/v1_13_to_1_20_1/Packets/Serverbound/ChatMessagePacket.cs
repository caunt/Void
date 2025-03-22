﻿using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class ChatMessagePacket : IServerboundPacket<ChatMessagePacket>
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
