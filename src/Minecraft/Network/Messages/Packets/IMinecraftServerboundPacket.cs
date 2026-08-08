namespace Void.Minecraft.Network.Messages.Packets;

// Do not use any of this anywhere except plugins

/// <summary>
/// Identifies a packet that travels toward the Minecraft server.
/// </summary>
public interface IMinecraftServerboundPacket : IMinecraftPacket;

/// <summary>
/// Identifies a decodable serverbound packet with a concrete result type.
/// </summary>
/// <typeparam name="T">The concrete packet type returned by decoding.</typeparam>
public interface IMinecraftServerboundPacket<out T> : IMinecraftServerboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;
