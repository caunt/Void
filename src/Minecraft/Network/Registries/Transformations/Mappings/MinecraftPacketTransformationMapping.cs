namespace Void.Minecraft.Network.Registries.Transformations.Mappings;

/// <summary>
/// Associates a packet transformation with its source and destination protocol versions.
/// </summary>
/// <param name="From">The version whose packet layout is consumed.</param>
/// <param name="To">The version whose packet layout is produced.</param>
/// <param name="Transformation">The field transformation to apply.</param>
public record MinecraftPacketTransformationMapping(ProtocolVersion From, ProtocolVersion To, MinecraftPacketTransformation Transformation);
