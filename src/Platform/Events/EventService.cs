using System.Reflection;
using Void.Common.Events;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Events;

public class EventService(ILogger<EventService> logger, IServiceProvider services, IHostApplicationLifetime hostApplicationLifetime) : IEventService
{
    private readonly List<Entry> _entries = [];

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
        var entries = _entries.OrderBy(entry => entry.Order);
        await ThrowAsync(entries, @event, cancellationToken);
    }

    private async ValueTask ThrowAsync<T>(IEnumerable<Entry> entriesNotSafe, T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        Entry[] entries;

        lock (this)
            entries = [.. entriesNotSafe];

        var eventType = @event.GetType();
        logger.LogTrace("Invoking {TypeName} event", eventType.Name);

        var simpleParameters = (object[])[@event];
        var cancellableParameters = (object[])[@event, CancellationTokenSource.CreateLinkedTokenSource(hostApplicationLifetime.ApplicationStopping, cancellationToken).Token];

        foreach (var entry in entries)
        {
            var parameters = entry.Method.GetParameters();

            if (!eventType.IsAssignableTo(parameters[0].ParameterType))
                continue;

            await Task.Yield();

            try
            {
                var value = entry.Method.Invoke(entry.Listener, parameters.Length == 1 ? simpleParameters : cancellableParameters);
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

    [Obsolete("Use IDependencyService.CreateInstance<TService>() instead")]
    public T RegisterListener<T>(params object[] parameters) where T : IEventListener
    {
        var instance = ActivatorUtilities.CreateInstance<T>(services, parameters);
        RegisterListeners(instance);
        return instance;
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

    private record Entry(IEventListener Listener, MethodInfo Method, PostOrder Order);
}
