﻿using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class SetCompressionPacket : IMinecraftClientboundPacket<SetCompressionPacket>
{
    public int Threshold { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(Threshold);
    }

    public static SetCompressionPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new SetCompressionPacket { Threshold = buffer.ReadVarInt() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
