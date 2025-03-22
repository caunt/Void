namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftStreamBase : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public bool IsAlive { get; }
    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}