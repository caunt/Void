namespace Void.Minecraft.Network.Registries.PacketId.Mappings;

public record MinecraftPacketIdMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);
