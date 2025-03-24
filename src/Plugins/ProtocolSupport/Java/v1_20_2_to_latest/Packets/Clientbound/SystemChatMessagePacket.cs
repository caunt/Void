using Void.Nbt;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    private NbtFile? _nbt;

    public required string Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
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

        buffer.WriteBoolean(Overlay);
    }

    public static SystemChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var data = buffer.ReadToEnd().ToArray();
        var nbt = NbtFile.Parse(data, false);

        buffer.Seek((int)nbt.Serialize().Length + 1);
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