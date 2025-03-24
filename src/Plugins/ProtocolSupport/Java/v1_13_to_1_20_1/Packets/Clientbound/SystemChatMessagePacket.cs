using Void.Nbt;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    private NbtFile? _nbt;

    public required string Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            if (_nbt is null)
            {
                var nbt = new NbtWriter();

                nbt.Write(NbtTagType.String);
                nbt.Write(Message);

                buffer.Write(nbt.GetStream());
            }
            else
            {
                buffer.Write(_nbt.Serialize());
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
        var nbt = (NbtFile?)null;
        var message = string.Empty;
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            var data = buffer.ReadToEnd().ToArray();
            nbt = NbtFile.Parse(data, false);

            buffer.Seek((int)nbt.Serialize().Length + 1);
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