using Void.Proxy.Plugins.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.Common.Packets;

public interface IClientboundPacket : IMinecraftPacket;

public interface IClientboundPacket<out T> : IClientboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;