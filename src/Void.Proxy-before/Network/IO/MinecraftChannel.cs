using System.Net.Sockets;
using Void.Proxy.Network.IO.Common;
using Void.Proxy.Network.IO.Compression;
using Void.Proxy.Network.IO.Encryption;

namespace Void.Proxy.Network.IO;

public class MinecraftChannel(Stream baseStream)
{
    public bool CanRead => baseStream.CanRead;
    public bool CanWrite => baseStream.CanWrite;

    private PacketStream packetStream = new(baseStream);

    public void EnableEncryption(byte[] secret)
    {
        if (baseStream is not NetworkStream)
            throw new InvalidOperationException($"Encryption can be enabled only on NetworkStream but channel have {baseStream.GetType()}");

        baseStream = new AesCfb8Stream(baseStream, secret);
        packetStream = new PacketStream(baseStream);
    }

    public void EnableCompression(int threshold) // this is very slow, needs to optimize
    {
        if (baseStream is DeprecatedCompressionStream)
            throw new InvalidOperationException($"Compression already enabled");

        baseStream = new DeprecatedCompressionStream(baseStream, threshold);
        packetStream = new PacketStream(baseStream);
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await packetStream.FlushAsync(cancellationToken);
    }

    public async ValueTask<MinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return await packetStream.ReadPacketAsync(cancellationToken);
    }

    public async ValueTask WriteMessageAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        await packetStream.WritePacketAsync(message, cancellationToken);
    }
}