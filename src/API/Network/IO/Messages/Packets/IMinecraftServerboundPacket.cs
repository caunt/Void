namespace Void.Proxy.Api.Network.IO.Messages.Packets;

// Do not use any of this anywhere except plugins

public interface IMinecraftServerboundPacket : IMinecraftPacket;

public interface IMinecraftServerboundPacket<out T> : IMinecraftServerboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;