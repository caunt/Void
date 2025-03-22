using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class LoginPluginRequestPacket : IClientboundPacket<LoginPluginRequestPacket>
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