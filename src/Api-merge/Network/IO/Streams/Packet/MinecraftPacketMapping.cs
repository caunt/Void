using Void.Proxy.Api.Mojang.Minecraft.Network;

namespace Void.Proxy.Api.Network.IO.Streams.Packet;

public record MinecraftPacketMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);