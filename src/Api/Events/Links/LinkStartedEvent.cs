using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Events.Links;

public record LinkStartedEvent(ILink Link) : IEvent;
