namespace Void.Minecraft.Network.Streams.Packet;

public record MinecraftPacketIdMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);
