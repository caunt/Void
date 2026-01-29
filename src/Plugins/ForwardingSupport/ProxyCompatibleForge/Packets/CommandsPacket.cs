using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ForwardingSupport.ProxyCompatibleForge.Packets;

public class CommandsPacket : IMinecraftClientboundPacket<CommandsPacket>
{
    public required byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.Write(Data);
    }

    public static CommandsPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new CommandsPacket
        {
            Data = buffer.ReadToEnd().ToArray()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
