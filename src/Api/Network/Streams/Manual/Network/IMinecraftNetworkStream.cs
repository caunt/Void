using System.Net.Sockets;
using Void.Common.Network.Streams;
using Void.Proxy.Api.Network.Streams.Manual;

namespace Void.Proxy.Api.Network.Streams.Manual.Network;

public interface IMinecraftNetworkStream : IManualStream, INetworkStreamBase
{
    public NetworkStream BaseStream { get; }
    public void PrependBuffer(Memory<byte> buffer);
}
