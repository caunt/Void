using System.Text;
using Void.Minecraft.Nbt;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    private NbtTag? _nbt;

    public required string Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (_nbt is null)
        {
            var data = Encoding.UTF8.GetBytes(Message);

            // NbtTagType.String
            buffer.WriteUnsignedByte(0x08);
            buffer.WriteUnsignedShort((ushort)data.Length);
            buffer.Write(data);
        }
        else
        {
            buffer.Write(_nbt.AsStream());
        }

        buffer.WriteBoolean(Overlay);
    }

    public static SystemChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var data = buffer.ReadToEnd().ToArray();
        var length = NbtTag.Parse(data, out var nbt);

        buffer.Seek(length);
        var overlay = buffer.ReadBoolean();

        return new SystemChatMessagePacket
        {
            Message = string.Empty,
            Overlay = overlay,
            _nbt = nbt
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}