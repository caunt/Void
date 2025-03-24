using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ExamplePlugin.Packets.Clientbound;

public class SetHeldItemClientboundPacket : IMinecraftClientboundPacket<SetHeldItemClientboundPacket>
{
    public required int Slot { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_21)
            buffer.WriteVarInt(Slot);
        else
            buffer.WriteUnsignedByte((byte)Slot);
    }

    public static SetHeldItemClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        int slot;

        if (protocolVersion > ProtocolVersion.MINECRAFT_1_21)
            slot = buffer.ReadVarInt();
        else
            slot = buffer.ReadUnsignedByte();

        return new SetHeldItemClientboundPacket
        {
            Slot = slot
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}