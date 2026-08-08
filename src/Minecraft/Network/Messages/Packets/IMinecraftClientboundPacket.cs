namespace Void.Minecraft.Network.Messages.Packets;

// Do not use any of this anywhere except plugins

/// <summary>
/// Identifies a packet that travels toward the Minecraft client.
/// </summary>
public interface IMinecraftClientboundPacket : IMinecraftPacket;

/// <summary>
/// Identifies a decodable clientbound packet with a concrete result type.
/// </summary>
/// <typeparam name="T">The concrete packet type returned by decoding.</typeparam>
public interface IMinecraftClientboundPacket<out T> : IMinecraftClientboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;
