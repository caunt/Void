namespace Void.Minecraft.Network.Registries.PacketId.Mappings;

/// <summary>
/// Associates a packet identifier with an inclusive protocol-version interval.
/// </summary>
/// <param name="Id">The numeric packet identifier.</param>
/// <param name="ProtocolVersion">The first protocol version for which the identifier is valid.</param>
/// <param name="LastValidProtocolVersion">The optional final valid version; <see langword="null" /> lets registry construction infer the end from the next mapping or latest version.</param>
public record MinecraftPacketIdMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null);
