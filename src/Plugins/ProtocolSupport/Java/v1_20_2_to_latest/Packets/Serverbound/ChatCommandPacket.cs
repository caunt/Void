using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class ChatCommandPacket : IServerboundPacket<ChatCommandPacket>
{
    public required string Command { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Command);
    }

    public static ChatCommandPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ChatCommandPacket { Command = buffer.ReadString() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}