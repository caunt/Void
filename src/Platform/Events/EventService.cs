using System.Reflection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Services;

namespace Void.Proxy.Events;

public class EventService(ILogger<EventService> logger) : IEventService
{
    private readonly List<IEventListener> _listeners = [];
    private readonly List<MethodInfo> _methods = [];

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
        logger.LogTrace("Invoking {TypeName} event", eventType.Name);

        var simpleParameters = (object[]) [@event];
        var cancellableParameters = (object[]) [@event, cancellationToken];

        for (var methodIndex = _methods.Count - 1; methodIndex >= 0; methodIndex--)
        {
            if (_methods.Count <= methodIndex)
                continue; // methods may change after event invocation

            var method = _methods[methodIndex];
            var parameters = method.GetParameters();

            if (parameters[0].ParameterType != eventType)
                continue;

            for (var listenerIndex = _listeners.Count - 1; listenerIndex >= 0; listenerIndex--)
            {
                if (_listeners.Count <= listenerIndex)
                    continue; // listeners may change after event invocation

                var listener = _listeners[listenerIndex];

                if (listener.GetType() != method.DeclaringType)
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
                    logger.LogError(exception.InnerException, "{EventName} cannot be invoked on {ListenerName}", eventType.Name, listener.GetType().FullName);
                }
            }
        }

        logger.LogTrace("Completed invoking {TypeName} event", eventType.Name);
    }

    public void RegisterListeners(params IEventListener[] listeners)
    {
        foreach (var listener in listeners)
        {
            logger.LogTrace("Registering {ListenerName} event listener", listener.GetType().Name);

            var methods = listener.GetType().GetMethods().Where(method => Attribute.IsDefined(method, typeof(SubscribeAttribute))).ToArray();

            foreach (var method in methods)
            {
                SubscribeAttribute.SanityChecks(method);
                _methods.Add(method);
            }
        }

        _listeners.AddRange(listeners);
    }

    public void UnregisterListeners(params IEventListener[] listeners)
    {
        foreach (var listener in listeners)
        {
            logger.LogTrace("Unregistering {ListenerName} event listener", listener.GetType().Name);

            var assembly = listener.GetType().Assembly;

            _listeners.Remove(listener);
            _methods.RemoveAll(method => method.DeclaringType?.Assembly == assembly);
        }
    }
}