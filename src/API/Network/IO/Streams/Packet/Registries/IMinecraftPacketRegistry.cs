using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

public interface IMinecraftPacketRegistry
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message);
    public bool Contains(Type type);
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public IMinecraftPacketRegistry ReplacePackets(IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketRegistry AddPackets(IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}