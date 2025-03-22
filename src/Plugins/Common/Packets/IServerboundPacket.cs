using Void.Proxy.Plugins.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.Common.Packets;

public interface IServerboundPacket : IMinecraftPacket;

public interface IServerboundPacket<out T> : IServerboundPacket, IMinecraftPacket<T> where T : class, IMinecraftPacket;