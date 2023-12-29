using Void.Proxy.API.Network.IO;
using Void.Proxy.API.Network.IO.Streams;

namespace Void.Proxy.Network.IO;

public class MinecraftChannel(IPacketStream baseStream)
{
    private IPacketStream _packetStream = baseStream;

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