using Void.NBT;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class PlayDisconnectPacket : IMinecraftClientboundPacket<PlayDisconnectPacket>
{
    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_20_3)
        {
            buffer.WriteString(Reason);
        }
        else
        {
            var nbt = new NbtWriter();
            nbt.Write(NbtTagType.String);
            nbt.Write(Reason);

            buffer.Write(nbt.GetStream());
        }
    }

    public static PlayDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_20_3)
        {
            return new PlayDisconnectPacket { Reason = buffer.ReadString() };
        }
        else
        {
            var data = buffer.ReadToEnd().ToArray();
            var nbt = new NbtReader(data);
            var type = nbt.ReadTagType();

            if (type is not NbtTagType.String)
                throw new NotSupportedException($"Only String NBT tag supported now ({type})");

            return new PlayDisconnectPacket { Reason = nbt.ReadString() };
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}