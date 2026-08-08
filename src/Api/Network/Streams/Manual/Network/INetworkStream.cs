using System.Net.Sockets;

namespace Void.Proxy.Api.Network.Streams.Manual.Network;

/// <summary>
/// Exposes direct proxy byte I/O over a TCP network stream.
/// </summary>
public interface INetworkStream : IManualStream, IMessageStreamBase
{
    /// <summary>
    /// Gets the underlying TCP network stream.
    /// </summary>
    public NetworkStream BaseStream { get; }

    /// <summary>
    /// Inserts bytes so they are returned before subsequently read network data.
    /// </summary>
    /// <param name="buffer">The bytes to prepend. The implementation can retain this memory until it has been consumed.</param>
    public void PrependBuffer(Memory<byte> buffer);
}
