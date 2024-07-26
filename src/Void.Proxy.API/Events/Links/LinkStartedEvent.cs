using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Links;

public class LinkStartedEvent : IEvent
{
    public required ILink Link { get; init; }
}