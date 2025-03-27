using System.Text;
using Void.Minecraft.Nbt;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class ConfigurationDisconnectPacket : IMinecraftClientboundPacket<ConfigurationDisconnectPacket>
{
    private NbtTag? _nbt;

    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
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

    public static ConfigurationDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var data = buffer.ReadToEnd().ToArray();
        NbtTag.Parse(data, out var nbt);

        return new ConfigurationDisconnectPacket { Reason = string.Empty, _nbt = nbt };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}