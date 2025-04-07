namespace Void.Minecraft.Network.Registries.Transformations;

public record MinecraftPacketTransformationMapping(ProtocolVersion From, ProtocolVersion To, MinecraftPacketTransformation Transformation);
