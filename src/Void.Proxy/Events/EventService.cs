using System.Reflection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Services;

namespace Void.Proxy.Events;

public class EventService : IEventService
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

        for (var i = 0; i < _methods.Count; i++)
        {
            var method = _methods[i];
            var parameters = method.GetParameters();

            if (parameters[0].ParameterType != eventType)
                continue;

            for (var index = 0; index < _listeners.Count; index++)
            {
                var listener = _listeners[index];

                if (listener.GetType() != method.DeclaringType)
                    continue;

                var value = method.Invoke(listener, parameters.Length == 1 ? [@event] : [@event, cancellationToken]);
                var handle = value switch
                {
                    Task task => new ValueTask(task),
                    ValueTask task => task,
                    _ => ValueTask.CompletedTask
                };

                await Task.Yield();
                await handle;
            }
        }
    }

    public void RegisterListeners(params IEventListener[] listeners)
    {
        foreach (var listener in listeners)
        {
            var methods = listener.GetType()
                .GetMethods()
                .Where(method => Attribute.IsDefined(method, typeof(SubscribeAttribute)))
                .ToArray();

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
            var assembly = listener.GetType()
                .Assembly;

            _listeners.Remove(listener);
            _methods.RemoveAll(method => method.DeclaringType?.Assembly == assembly);
        }
    }
}