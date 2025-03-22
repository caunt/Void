using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;

namespace Void.Proxy.Plugins.Common.Network.Protocol.Packets;

public record PacketMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);