using System.Text;
using Void.Minecraft.Nbt;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    private NbtTag? _nbt;

    public required string Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
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
        }
        else
        {
            buffer.WriteString(Message);
        }

        if (protocolVersion > ProtocolVersion.MINECRAFT_1_19)
            buffer.WriteBoolean(Overlay);
    }

    public static SystemChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var nbt = (NbtTag?)null;
        var message = string.Empty;
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            var data = buffer.ReadToEnd().ToArray();
            var length = NbtTag.Parse(data, out nbt);

            buffer.Seek(length);
        }
        else
        {
            message = buffer.ReadString();
        }

        var overlay = false;
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_19)
            overlay = buffer.ReadBoolean();

        return new SystemChatMessagePacket
        {
            Message = message,
            Overlay = overlay,
            _nbt = nbt
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}