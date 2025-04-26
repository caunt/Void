using System.Runtime.CompilerServices;
using Nito.AsyncEx;

namespace Void.Proxy.Api.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Synchronized<T>(this IEnumerable<T> source, AsyncLock @lock, CancellationToken cancellationToken)
    {
        using var _ = @lock.Lock(cancellationToken);

        foreach (var item in source)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> SynchronizedAsync<T>(this IEnumerable<T> source, AsyncLock @lock, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var _ = await @lock.LockAsync(cancellationToken);

        foreach (var item in source)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return item;
        }
    }
}
