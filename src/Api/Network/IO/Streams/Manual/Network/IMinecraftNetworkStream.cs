using System.Net.Sockets;
using Void.Minecraft.Network.Streams;

namespace Void.Proxy.Api.Network.IO.Streams.Manual.Network;

public interface IMinecraftNetworkStream : IMinecraftManualStream, IMinecraftStreamBase
{
    public NetworkStream BaseStream { get; }
    public void PrependBuffer(Memory<byte> buffer);
}
