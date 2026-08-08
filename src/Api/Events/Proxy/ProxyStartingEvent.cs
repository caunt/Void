namespace Void.Proxy.Api.Events.Proxy;

/// <summary>
/// Signals that the proxy is beginning its startup sequence.
/// </summary>
/// <remarks>
/// This event is published before hosted proxy services have completed startup.
/// </remarks>
public record ProxyStartingEvent : IEvent;
