using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol.States;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets;

public interface IMinecraftPacket<in T> : IMinecraftPacket where T : IProtocolState
{
    Task<bool> HandleAsync(T state);
}