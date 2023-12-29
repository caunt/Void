namespace Void.Proxy.API.Network.IO;

public interface IMinecraftChannel
{
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default);

    public ValueTask<MinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default);

    public ValueTask WriteMessageAsync(MinecraftMessage message, CancellationToken cancellationToken = default);
}