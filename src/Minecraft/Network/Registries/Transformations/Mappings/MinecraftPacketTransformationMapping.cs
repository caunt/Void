namespace Void.Minecraft.Network.Registries.Transformations.Mappings;

public record MinecraftPacketTransformationMapping(ProtocolVersion From, ProtocolVersion To, MinecraftPacketTransformation Transformation);
