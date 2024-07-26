using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Links;

public class LinkStartingEvent : IEvent
{
    public required ILink Link { get; init; }
}