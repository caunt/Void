using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;

namespace Void.Proxy.Plugins.Common.Registries.Packets;

public interface IPacketRegistry
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out PacketDecoder<IMinecraftPacket> packet);
    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id);
    public IPacketRegistry ReplacePackets(IReadOnlyDictionary<PacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IPacketRegistry AddPackets(IReadOnlyDictionary<PacketMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}