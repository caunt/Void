using System.Collections.Concurrent;
using System.Reflection;
using DryIoc;
using Nito.Disposables.Internals;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Events;

public class EventService(ILogger<EventService> logger, IContainer container) : IEventService
{
    private readonly List<Entry> _entries = [];
    private readonly ConcurrentDictionary<Type, bool> _scopedEventTypes = [];

    public IEnumerable<IEventListener> Listeners => [.. _entries.Select(entry => entry.Listener)];

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
        var entries = _entries.WhereNotNull().OrderBy(entry => entry.Order).Select(entry => new WeakEntry(new WeakReference<IEventListener>(entry.Listener), new WeakReference<MethodInfo>(entry.Method), entry.Order));
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
        var eventType = @event.GetType();
        logger.LogTrace("Invoking {TypeName} event", eventType.Name);

        var simpleParameters = (object[])[@event];
        var cancellableParameters = (object[])[@event, cancellationToken];

        var isScoped = _scopedEventTypes.GetOrAdd(eventType, type =>
        {
            var matches = container
                .GetServiceRegistrations()
                .Where(registration => type.IsAssignableTo(registration.ServiceType))
                .ToArray();

            if (matches.Length > 1)
                throw new NotSupportedException($"Duplicate service listening to {type.Name}: " + string.Join(", ", matches.Select(r => r.ServiceType)));

            return matches.Length == 1 && matches[0].Factory.Reuse == Reuse.Scoped;
        });

        foreach (var entry in entries)
        {
            var listener = entry.Listener;
            var method = entry.Method;

            if (listener is null || method is null)
                continue;

            var parameters = method.GetParameters();

            if (!entry.IsCompatible(eventType))
                continue;

            await Task.Yield();

            try
            {
                var value = method.Invoke(listener, parameters.Length == 1 ? simpleParameters : cancellableParameters);
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

    public void RegisterListeners(IEnumerable<IEventListener> listeners)
    {
        foreach (var listener in listeners)
            RegisterListeners(listener);
    }

    public void RegisterListeners(params IEventListener[] listeners)
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

                        var attribute = method.GetCustomAttribute<SubscribeAttribute>()!;
                        _entries.Add(new Entry(listener, method, attribute.Order));
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

    private record WeakEntry(WeakReference<IEventListener> ListenerReference, WeakReference<MethodInfo> MethodReference, PostOrder Order)
    {
        public IEventListener? Listener => ListenerReference.TryGetTarget(out var listener) ? listener : null;
        public MethodInfo? Method => MethodReference.TryGetTarget(out var method) ? method : null;

        public bool IsCompatible(Type eventType) => MethodReference.TryGetTarget(out var method) && eventType.IsAssignableTo(method.GetParameters()[0].ParameterType);
    }

    private record Entry(IEventListener Listener, MethodInfo Method, PostOrder Order);
}
