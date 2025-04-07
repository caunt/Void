using System.Net.Sockets;
using Void.Common.Network.Streams;

namespace Void.Proxy.Api.Network.IO.Streams.Manual.Network;

public interface IMinecraftNetworkStream : IManualStream, INetworkStreamBase
{
    public NetworkStream BaseStream { get; }
    public void PrependBuffer(Memory<byte> buffer);
}
