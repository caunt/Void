using Nito.Disposables.Internals;
using System.Reflection;
using Void.Proxy.API.Events;

namespace Void.Proxy.Events;

public class EventManager : IEventManager
{
    private readonly List<IEventListener> _listeners = [];
    private readonly List<MethodInfo> _methods = [];

    internal async Task ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new()
    {
        await ThrowAsync(new T(), cancellationToken);
    }

    public async Task ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var eventType = typeof(T);

        foreach (var method in _methods)
        {
            var parameters = method.GetParameters();

            if (parameters[0].ParameterType != eventType)
                continue;

            var value = method.Invoke(_listeners.First(listener => listener.GetType() == method.DeclaringType), parameters.Length == 1 ? [@event] : [@event, cancellationToken]);
            var handle = value switch
            {
                Task task => new ValueTask(task),
                ValueTask task => task,
                _ => ValueTask.CompletedTask
            };

            await handle;
        }
    }

    public IEventListener[] RegisterListeners(Assembly assembly)
    {
        var listenerInterface = typeof(IEventListener);
        var listeners = assembly.GetTypes()
                .Where(listenerInterface.IsAssignableFrom)
                .Select(CreateListenerInstance)
                .Cast<IEventListener?>()
                .WhereNotNull()
                .ToArray();

        foreach (var listener in listeners)
        {
            var methods = listener.GetType().GetMethods()
                .Where(method => Attribute.IsDefined(method, typeof(SubscribeAttribute)))
                .ToArray();

            foreach (var method in methods)
            {
                SubscribeAttribute.SanityChecks(method);
                _methods.Add(method);
            }
        }

        _listeners.AddRange(listeners);
        return listeners;
    }

    public void UnregisterListeners(IEventListener[] listeners)
    {
        foreach (var listener in listeners)
        {
            var assembly = listener.GetType().Assembly;

            _listeners.Remove(listener);
            _methods.RemoveAll(method => method.DeclaringType?.Assembly == assembly);
        }
    }

    private object? CreateListenerInstance(Type type)
    {
        if (Proxy.Plugins.GetExistingInstance(type) is { } instance)
            return instance;

        return Activator.CreateInstance(type);
    }
}
