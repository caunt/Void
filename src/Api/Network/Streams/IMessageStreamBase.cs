namespace Void.Proxy.Api.Network.Streams;

public interface IMessageStreamBase : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public bool IsAlive { get; }
    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}
