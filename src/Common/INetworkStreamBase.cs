using System;
using System.Threading;
using System.Threading.Tasks;

namespace Void.Common;

public interface INetworkStreamBase : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public bool IsAlive { get; }
    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}
