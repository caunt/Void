namespace Void.Proxy.API.Network.Protocol;

public record PacketMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);