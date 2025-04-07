namespace Void.Minecraft.Network.Messages.Packets;

// Do not use any of this anywhere except plugins

public interface IMinecraftServerboundPacket : IMinecraftPacket;

public interface IMinecraftServerboundPacket<out T> : IMinecraftServerboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;
