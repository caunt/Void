namespace Void.Minecraft.Network.Messages.Packets;

// Do not use any of this anywhere except plugins

public interface IMinecraftClientboundPacket : IMinecraftPacket;

public interface IMinecraftClientboundPacket<out T> : IMinecraftClientboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;
