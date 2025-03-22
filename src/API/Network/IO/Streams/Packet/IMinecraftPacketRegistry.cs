using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.API.Network.IO.Streams.Packet;

public interface IMinecraftPacketRegistry
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public IMinecraftPacketRegistry ReplacePackets(IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketRegistry AddPackets(IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}