namespace Void.Proxy.Plugins.API.Events;

[AttributeUsage(AttributeTargets.Method)]
public class SubscribeAttribute<T> : Attribute where T : IEvent
{
}