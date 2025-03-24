using Void.Nbt;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class ConfigurationDisconnectPacket : IMinecraftClientboundPacket<ConfigurationDisconnectPacket>
{
    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var nbt = new NbtWriter();
        nbt.Write(NbtTagType.String);
        nbt.Write(Reason);

        buffer.Write(nbt.GetStream());
    }

    public static ConfigurationDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var data = buffer.ReadToEnd().ToArray();
        var nbt = new NbtReader(data);
        var type = nbt.ReadTagType();

        if (type is not NbtTagType.String)
            throw new NotSupportedException($"Only String NBT tag supported now ({type})");

        return new ConfigurationDisconnectPacket { Reason = nbt.ReadString() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}