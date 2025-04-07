using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ExamplePlugin.Packets.Serverbound;

public class SetHeldItemServerboundPacket : IMinecraftServerboundPacket<SetHeldItemServerboundPacket>
{
    public required ushort Slot { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUnsignedShort(Slot);
    }

    public static SetHeldItemServerboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new SetHeldItemServerboundPacket
        {
            Slot = buffer.ReadUnsignedShort()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
