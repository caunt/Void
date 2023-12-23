using Void.Proxy.Network.IO.Common;

namespace Void.Proxy.Network.IO;

public class MinecraftChannel(Stream baseStream)
{
    public bool CanRead => baseStream.CanRead;
    public bool CanWrite => baseStream.CanWrite;

    private PacketStream _packetStream = new(baseStream);

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await _packetStream.FlushAsync(cancellationToken);
    }

    public async ValueTask<MinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return await _packetStream.ReadPacketAsync(cancellationToken);
    }

    public async ValueTask WriteMessageAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        await _packetStream.WritePacketAsync(message, cancellationToken);
    }
}