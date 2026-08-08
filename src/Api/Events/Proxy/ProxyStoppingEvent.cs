namespace Void.Proxy.Api.Events.Proxy;

/// <summary>
/// Signals that the proxy is beginning its shutdown sequence.
/// </summary>
public record ProxyStoppingEvent : IEvent;
