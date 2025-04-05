using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

public interface IMinecraftPacketTransformations
{
    public IEnumerable<Type> PacketTypes { get; }
    public bool IsEmpty { get; }

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public bool TryGetTransformation(IMinecraftPacket packet, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations);
    public IMinecraftPacketTransformations ReplaceTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public IMinecraftPacketTransformations AddTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> mappings, ProtocolVersion protocolVersion);
    public void Clear();
}
