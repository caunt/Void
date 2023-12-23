using Nito.Disposables.Internals;
using System.Reflection;
using Void.Proxy.API.Events;

namespace Void.Proxy.Events;

public class EventManager : IEventManager
{
    private readonly List<IEventListener> _listeners = [];
    private readonly List<MethodInfo> _methods = [];

    public async Task ThrowAsync<T>(T @event) where T : IEvent
    {
        var eventType = typeof(T);

        foreach (var method in _methods)
        {
            var parameters = method.GetParameters();

            if (parameters[0].ParameterType != eventType)
                continue;

            var value = method.Invoke(_listeners.First(listener => listener.GetType() == method.DeclaringType), [@event]);

            if (value is Task task)
                await task;
        }
    }

    public IEventListener[] RegisterListeners(Assembly assembly)
    {
        var listenerInterface = typeof(IEventListener);
        var listeners = assembly.GetTypes()
                .Where(listenerInterface.IsAssignableFrom)
                .Select(Activator.CreateInstance)
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
}
