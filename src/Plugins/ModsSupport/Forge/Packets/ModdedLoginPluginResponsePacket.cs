using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ModsSupport.Forge.Packets;

public class ModdedLoginPluginResponsePacket : IMinecraftServerboundPacket<ModdedLoginPluginResponsePacket>
{
    public required int MessageId { get; set; }
    public required bool Successful { get; set; }
    public required byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBoolean(Successful);
        buffer.Write(Data);
    }

    public static ModdedLoginPluginResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ModdedLoginPluginResponsePacket
        {
            MessageId = buffer.ReadVarInt(),
            Successful = buffer.ReadBoolean(),
            Data = buffer.ReadToEnd().ToArray()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
