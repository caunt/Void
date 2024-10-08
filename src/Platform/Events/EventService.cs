﻿using System.Reflection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Services;

namespace Void.Proxy.Events;

public class EventService(ILogger<EventService> logger, IServiceProvider services) : IEventService
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
        var eventType = @event.GetType();
        logger.LogTrace("Invoking {TypeName} event", eventType.Name);

        var simpleParameters = (object[]) [@event];
        var cancellableParameters = (object[]) [@event, cancellationToken];

        var entries = _entries.OrderByDescending(entry => entry.Order).ToArray();

        foreach (var entry in entries)
        {
            var parameters = entry.Method.GetParameters();

            if (parameters[0].ParameterType != eventType)
                continue;

            if (entry.Listener.GetType() != entry.Method.DeclaringType)
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
        foreach (var listener in listeners)
        {
            logger.LogTrace("Registering {ListenerType} event listener", listener);

            var methods = listener.GetType().GetMethods().Where(method => Attribute.IsDefined(method, typeof(SubscribeAttribute)));

            foreach (var method in methods)
            {
                SubscribeAttribute.SanityChecks(method);

                var attribute = method.GetCustomAttribute<SubscribeAttribute>()!;
                _entries.Add(new Entry(listener, method, attribute.Order));
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

    private record Entry(IEventListener Listener, MethodInfo Method, PostOrder Order);
}