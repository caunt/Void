namespace Void.Proxy.Api.Events;

/// <summary>
/// Identifies an object whose subscribed methods can receive proxy events.
/// </summary>
/// <remarks>
/// Methods are discovered when the listener is registered with the event service and must be annotated with <see cref="SubscribeAttribute" />.
/// </remarks>
public interface IEventListener;
