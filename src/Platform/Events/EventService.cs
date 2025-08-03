using System.Reflection;
using DryIoc;
using Nito.Disposables.Internals;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Events;

public class EventService(ILogger<EventService> logger, IContainer container) : IEventService
{
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
        var entries = _entries
            .WhereNotNull()
            .OrderBy(entry => entry.Order)
            .Select(entry => new WeakEntry(new WeakReference<IEventListener>(entry.Listener), new WeakReference<MethodInfo>(entry.Method), entry.Order, entry.BypassScopedFilter, entry.CancellationToken));

        await ThrowAsync(entries, @event, cancellationToken);
    }

    private async ValueTask ThrowAsync<T>(IEnumerable<WeakEntry> entriesNotSafe, T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var eventType = @event.GetType();
        var alreadyInvoked = new HashSet<WeakEntry>();

        while (true)
        {
            var toInvoke = entriesNotSafe
                .Where(entry => entry.IsCompatible(eventType))
                .Where(entry => alreadyInvoked.All(invokedEntry => invokedEntry.Listener != entry.Listener || invokedEntry.Method != entry.Method))
                .Where(alreadyInvoked.Add)
                .ToArray();

            if (toInvoke.Length == 0)
                return;

            await ThrowOnceAsync(toInvoke, @event, cancellationToken);
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

                    // Allow invocation of scoped events only if scoped player matched
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
}
