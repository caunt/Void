namespace Void.Proxy.Api.Network.IO.Messages.Packets;

// Do not use any of this anywhere except plugins

public interface IMinecraftClientboundPacket : IMinecraftPacket;

public interface IMinecraftClientboundPacket<out T> : IMinecraftClientboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;
