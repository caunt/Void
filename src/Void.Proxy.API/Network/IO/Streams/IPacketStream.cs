namespace Void.Proxy.API.Network.IO.Streams;

public interface IPacketStream
{
    public Task FlushAsync(CancellationToken cancellationToken);

    public ValueTask<MinecraftMessage> ReadPacketAsync(CancellationToken cancellationToken = default);

    public ValueTask WritePacketAsync(MinecraftMessage message, CancellationToken cancellationToken = default);
}
