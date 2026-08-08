using System.Runtime.CompilerServices;
using Nito.AsyncEx;

namespace Void.Proxy.Api.Extensions;

/// <summary>
/// Provides enumeration helpers that hold an asynchronous-compatible lock for the duration of enumeration.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Enumerates a sequence while synchronously holding an <see cref="AsyncLock" />.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="source">The sequence to enumerate.</param>
    /// <param name="lock">The lock held from the first move until enumeration ends or is disposed.</param>
    /// <param name="cancellationToken">A token used to cancel lock acquisition and stop further enumeration.</param>
    /// <returns>A lazy sequence whose enumerator owns the lock while active.</returns>
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

    /// <summary>
    /// Asynchronously enumerates a sequence while holding an <see cref="AsyncLock" />.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="source">The sequence to enumerate.</param>
    /// <param name="lock">The lock held from the first move until asynchronous enumeration ends or is disposed.</param>
    /// <param name="cancellationToken">A token used to cancel lock acquisition and stop further enumeration.</param>
    /// <returns>A lazy asynchronous sequence whose enumerator owns the lock while active.</returns>
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
