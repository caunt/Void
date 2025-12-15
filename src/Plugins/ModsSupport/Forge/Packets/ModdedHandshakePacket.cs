using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ModsSupport.Forge.Packets;

public class ModdedHandshakePacket : IMinecraftServerboundPacket<ModdedHandshakePacket>
{
    private ReadOnlySpan<string> AddressParts => ServerAddress.Split('\0', StringSplitOptions.RemoveEmptyEntries);

    public bool IsForge => ForgeMarker.Range().Any(marker => AddressParts.Contains(marker.Name));
    public ReadOnlySpan<string> Markers => AddressParts.Length == 1 ? [] : AddressParts[1..]; // Remove the first part which is the actual address

    public required int ProtocolVersion { get; set; }
    public required string ServerAddress { get; set; }
    public required ushort ServerPort { get; set; }
    public required int NextState { get; set; }

    public static ModdedHandshakePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ModdedHandshakePacket
        {
            ProtocolVersion = buffer.ReadVarInt(),
            ServerAddress = buffer.ReadString(255 + ForgeMarker.Longest.Name.Length),
            ServerPort = buffer.ReadUnsignedShort(),
            NextState = buffer.ReadVarInt()
        };
    }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(ProtocolVersion);
        buffer.WriteString(ServerAddress);
        buffer.WriteUnsignedShort(ServerPort);
        buffer.WriteVarInt(NextState);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
