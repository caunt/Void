using Void.Proxy.API.Mojang.Minecraft.Network;

namespace Void.Proxy.API.Network.IO.Streams.Packet;

public record MinecraftPacketMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);