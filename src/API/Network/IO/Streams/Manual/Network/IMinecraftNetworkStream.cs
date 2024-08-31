using System.Net.Sockets;

namespace Void.Proxy.API.Network.IO.Streams.Manual.Network;

public interface IMinecraftNetworkStream : IMinecraftManualStream, IMinecraftStreamBase
{
    public NetworkStream BaseStream { get; }
    public void PrependBuffer(Memory<byte> buffer);
}