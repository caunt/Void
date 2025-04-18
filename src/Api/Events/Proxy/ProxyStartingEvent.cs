using Microsoft.Extensions.DependencyInjection;

namespace Void.Proxy.Api.Events.Proxy;

public record ProxyStartingEvent(ServiceCollection Services) : IEvent;
