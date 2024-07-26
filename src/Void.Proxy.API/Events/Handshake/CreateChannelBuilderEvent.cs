using System.Net.Sockets;
using Void.Proxy.API.Network.IO.Channels;

namespace Void.Proxy.API.Events.Handshake;

public class CreateChannelBuilderEvent : IEventWithResult<Func<NetworkStream, Task<IMinecraftChannel>>>
{
    public required Memory<byte> Buffer { get; init; }
    public Func<NetworkStream, Task<IMinecraftChannel>>? Result { get; set; }
}