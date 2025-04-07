namespace Void.Minecraft.Network.Registries.PacketId;

public record MinecraftPacketIdMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);
