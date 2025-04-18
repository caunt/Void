using System.Net.Sockets;

namespace Void.Proxy.Api.Network.Streams.Manual.Network;

public interface INetworkStream : IManualStream, IMessageStreamBase
{
    public NetworkStream BaseStream { get; }
    public void PrependBuffer(Memory<byte> buffer);
}
