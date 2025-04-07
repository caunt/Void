using Void.Minecraft.Network.Streams.Packet.Transformations;

namespace Void.Minecraft.Network.Streams.Packet;

public record MinecraftPacketTransformationMapping(ProtocolVersion From, ProtocolVersion To, MinecraftPacketTransformation Transformation);
