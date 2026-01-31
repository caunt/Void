using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using DryIoc;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Events;

public class EventService(ILogger<EventService> logger, IContainer container) : IEventService
{
    private readonly ConcurrentDictionary<Func<IEvent, bool>, TaskCompletionSource> _waiters = [];
    private readonly List<Entry> _entries = [];

    public IEnumerable<IEventListener> Listeners
    {
        get
        {
            var listeners = new IEventListener[_entries.Count];

            for (var i = 0; i < _entries.Count; i++)
                listeners[i] = _entries[i].Listener;

            return listeners;
        }
    }

    public async ValueTask ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new()
    {
        await ThrowAsync(new T(), cancellationToken);
    }

    public async ValueTask<TResult?> ThrowWithResultAsync<TResult>(IEventWithResult<TResult> @event, CancellationToken cancellationToken = default)
    {
        await ThrowAsync(@event, cancellationToken);
        return @event.Result;
    }

    public async ValueTask<TResult?> ThrowWithResultAsync<T, TResult>(CancellationToken cancellationToken = default) where T : IEventWithResult<TResult?>, new()
    {
        var @event = new T();
        await ThrowAsync(@event, cancellationToken);
        return @event.Result;
    }

    public async ValueTask ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var eventType = @event.GetType();

        var invoked = new HashSet<InvokedListenerMethod>(InvokedListenerMethodComparer.Instance);
        var firstParameterTypeCache = new Dictionary<MethodInfo, Type>(ReferenceEqualityComparer.Instance);

        // Entries list is often modified during invocation (listeners being registered/unregistered), iterate until no more candidates are found
        while (true)
        {
            var invocationCandidates = new List<InvocationCandidate>();

            var entriesCountSnapshot = _entries.Count;
            for (var entryIndex = 0; entryIndex < entriesCountSnapshot; entryIndex++)
            {
                var entry = _entries[entryIndex];

                var invokedKey = new InvokedListenerMethod(entry.Listener, entry.Method);

                if (invoked.Contains(invokedKey))
                    continue;

                if (!firstParameterTypeCache.TryGetValue(entry.Method, out var firstParameterType))
                {
                    firstParameterType = entry.Method.GetParameters()[0].ParameterType;
                    firstParameterTypeCache.Add(entry.Method, firstParameterType);
                }

                if (!eventType.IsAssignableTo(firstParameterType))
                    continue;

                invoked.Add(invokedKey);

                invocationCandidates.Add(new InvocationCandidate(
                    entry.Order,
                    entryIndex,
                    new WeakEntry(
                        new WeakReference<IEventListener>(entry.Listener),
                        new WeakReference<MethodInfo>(entry.Method),
                        entry.Order,
                        entry.BypassScopedFilter,
                        entry.CancellationToken)));
            }

            if (invocationCandidates.Count is 0)
                break;

            invocationCandidates.Sort(static (left, right) =>
            {
                var orderComparison = left.Order.CompareTo(right.Order);

                if (orderComparison is not 0)
                    return orderComparison;

                return left.Sequence.CompareTo(right.Sequence);
            });

            var toInvoke = new WeakEntry[invocationCandidates.Count];

            for (var candidateIndex = 0; candidateIndex < invocationCandidates.Count; candidateIndex++)
                toInvoke[candidateIndex] = invocationCandidates[candidateIndex].WeakEntry;

            await ThrowOnceAsync(toInvoke, @event, cancellationToken);
        }

        if (_waiters.IsEmpty)
            return;

        foreach (var (condition, taskCompletionSource) in _waiters.ToArray())
        {
            if (!condition(@event))
                continue;

            if (_waiters.TryRemove(condition, out _))
                taskCompletionSource.SetResult();
        }
    }

    private async ValueTask ThrowOnceAsync<T>(WeakEntry[] entries, T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var dependencies = container.Resolve<IDependencyService>();

        var eventType = @event.GetType();
        logger.LogTrace("Invoking {TypeName} event", eventType.Name);

        foreach (var entry in entries)
        {
            var listener = entry.Listener;
            var method = entry.Method;

            if (listener is null || method is null)
                continue;

            if (!entry.IsCompatible(eventType))
                continue;

            await Task.Yield();

            if (!entry.BypassScopedFilter && @event is IScopedEvent scopedEvent)
            {
                if (dependencies.TryGetScopedPlayerContext(listener, out var context))
                {
                    if (context.Player != scopedEvent.Player)
                        continue;

                    // Allow invocation of scoped events only if the scoped player matches
                }
            }

            var parameters = method.GetParameters();

            try
            {
                using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, entry.CancellationToken);
                var value = method.Invoke(listener, parameters.Length == 1 ? [@event] : [@event, cancellationTokenSource.Token]);
                var handle = value switch
                {
                    Task task => new ValueTask(task),
                    ValueTask task => task,
                    _ => ValueTask.CompletedTask
                };

                await handle;
            }
            catch (TargetInvocationException exception)
            {
                logger.LogError(exception.InnerException, "{EventName} cannot be invoked on {ListenerName}", eventType.Name, entry.Listener);
            }
        }

        logger.LogTrace("Completed invoking {TypeName} event", eventType.Name);
    }

    public async ValueTask WaitAsync(Func<IEvent, bool> condition, CancellationToken cancellationToken = default)
    {
        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _waiters[condition] = taskCompletionSource;

        using var registration = cancellationToken.Register(() =>
        {
            if (!_waiters.Remove(condition, out _))
                return;

            taskCompletionSource.SetCanceled();
        });

        await taskCompletionSource.Task;
    }

    public void RegisterListeners(IEnumerable<IEventListener> listeners, CancellationToken cancellationToken = default)
    {
        foreach (var listener in listeners)
            RegisterListeners(cancellationToken, listener);
    }

    public void RegisterListeners(params IEventListener[] listeners)
    {
        foreach (var listener in listeners)
            RegisterListeners(CancellationToken.None, listener);
    }

    public void RegisterListeners(CancellationToken cancellationToken = default, params IEventListener[] listeners)
    {
        lock (this)
        {
            foreach (var listener in listeners)
            {
                if (_entries.Any(entry => entry.Listener == listener))
                    continue;

                logger.LogTrace("Registering {Type} event listener", listener);

                var type = listener.GetType();
                while (type != null)
                {
                    var methods = type
                        .GetMethods(
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Static |
                            BindingFlags.Instance |
                            BindingFlags.DeclaredOnly)
                        .Where(method => Attribute.IsDefined(method, typeof(SubscribeAttribute)));

                    foreach (var method in methods)
                    {
                        logger.LogTrace("Registering {Type} event listener method {Name}", listener, method.Name);
                        SubscribeAttribute.SanityChecks(method);

                        var attribute = method.GetCustomAttribute<SubscribeAttribute>()
                                         ?? throw new InvalidOperationException($"Method {method.Name} does not define {nameof(SubscribeAttribute)}.");
                        _entries.Add(new Entry(listener, method, attribute.Order, attribute.BypassScopedFilter, cancellationToken));
                    }

                    type = type.BaseType;
                }
            }
        }
    }

    public void UnregisterListeners(IEnumerable<IEventListener> listeners)
    {
        foreach (var listener in listeners)
            UnregisterListeners(listener);
    }

    public void UnregisterListeners(params IEventListener[] listeners)
    {
        lock (this)
        {
            foreach (var listener in listeners)
            {
                logger.LogTrace("Unregistering {ListenerType} event listener", listener);

                for (var i = _entries.Count - 1; i >= 0; i--)
                {
                    if (_entries.Count <= i)
                        continue;

                    var entry = _entries[i];

                    if (entry.Listener != listener)
                        continue;

                    _entries.Remove(entry);
                }
            }
        }
    }

    private record WeakEntry(WeakReference<IEventListener> ListenerReference, WeakReference<MethodInfo> MethodReference, PostOrder Order, bool BypassScopedFilter, CancellationToken CancellationToken)
    {
        public IEventListener? Listener => ListenerReference.TryGetTarget(out var listener) ? listener : null;
        public MethodInfo? Method => MethodReference.TryGetTarget(out var method) ? method : null;

        public bool IsCompatible(Type eventType) => MethodReference.TryGetTarget(out var method) && eventType.IsAssignableTo(method.GetParameters()[0].ParameterType);
    }

    private record Entry(IEventListener Listener, MethodInfo Method, PostOrder Order, bool BypassScopedFilter, CancellationToken CancellationToken);

    private readonly record struct InvocationCandidate(PostOrder Order, int Sequence, WeakEntry WeakEntry);

    private readonly record struct InvokedListenerMethod(IEventListener Listener, MethodInfo Method);

    private sealed class InvokedListenerMethodComparer : IEqualityComparer<InvokedListenerMethod>
    {
        public static readonly InvokedListenerMethodComparer Instance = new();

        public bool Equals(InvokedListenerMethod left, InvokedListenerMethod right)
        {
            return ReferenceEquals(left.Listener, right.Listener) && ReferenceEquals(left.Method, right.Method);
        }

        public int GetHashCode(InvokedListenerMethod value)
        {
            return HashCode.Combine(RuntimeHelpers.GetHashCode(value.Listener), RuntimeHelpers.GetHashCode(value.Method));
        }
    }
}
