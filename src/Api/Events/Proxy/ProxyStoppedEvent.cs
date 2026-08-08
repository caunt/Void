namespace Void.Proxy.Api.Events.Proxy;

/// <summary>
/// Signals that the proxy has completed its shutdown sequence.
/// </summary>
public record ProxyStoppedEvent : IEvent;
