using System.Text;
using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class PlayDisconnectPacket : IMinecraftClientboundPacket<PlayDisconnectPacket>
{
    private NbtTag? _nbt;

    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_20_3)
        {
            buffer.WriteString(Reason);
        }
        else
        {
            if (_nbt is null)
            {
                var data = Encoding.UTF8.GetBytes(Reason);

                // NbtTagType.String
                buffer.WriteUnsignedByte(0x08);
                buffer.WriteUnsignedShort((ushort)data.Length);
                buffer.Write(data);
            }
            else
            {
                buffer.Write(_nbt.AsStream());
            }
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
            NbtTag.Parse(data, out var nbt);

            return new PlayDisconnectPacket { Reason = string.Empty, _nbt = nbt };
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}