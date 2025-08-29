using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ModsSupport.Forge.Packets;

public class PluginMessagePacket : IMinecraftPacket<PluginMessagePacket>
{
    public required string Channel { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Channel);
        WriteData(ref buffer);

        void WriteData(ref MinecraftBuffer buffer)
        {
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            {
                buffer.Write(Data.Span);
                return;
            }


            WriteExtendedForgeShort(ref buffer, Data.Length);
            buffer.Write(Data.Span);
        }
    }

    public static PluginMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var channel = buffer.ReadString();
        var data = ReadData(ref buffer);

        return new PluginMessagePacket
        {
            Channel = channel,
            Data = data
        };

        ReadOnlyMemory<byte> ReadData(ref MinecraftBuffer buffer)
        {
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
                return buffer.ReadToEnd().ToArray();

            var length = ReadExtendedForgeShort(ref buffer);
            return buffer.Read(length).ToArray();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private static int ReadExtendedForgeShort(ref MinecraftBuffer buffer)
    {
        var low = (int)buffer.ReadUnsignedShort();
        var high = 0;

        if ((low & 0x8000) != 0)
        {
            low &= 0x7FFF;
            high = buffer.ReadUnsignedByte() & 0xFF;
        }

        return ((high & 0xFF) << 15) | low;
    }

    private static void WriteExtendedForgeShort(ref MinecraftBuffer buffer, int toWrite)
    {
        var low = toWrite & 0x7FFF;
        var high = (toWrite & 0x7F8000) >> 15;

        if (high != 0)
            low |= 0x8000;

        buffer.WriteShort((short)low);

        if (high != 0)
            buffer.WriteUnsignedByte((byte)high);
    }
}
