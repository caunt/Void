﻿using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity.Packets;

public class LoginPluginRequestPacket : IMinecraftClientboundPacket<LoginPluginRequestPacket>
{
    public required int MessageId { get; set; }
    public required string Channel { get; set; }
    public required byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteString(Channel);
        buffer.Write(Data);
    }

    public static LoginPluginRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginPluginRequestPacket
        {
            MessageId = buffer.ReadVarInt(),
            Channel = buffer.ReadString(),
            Data = buffer.ReadToEnd().ToArray()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
