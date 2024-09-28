using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Common.Network.Protocol;

public record PacketMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);