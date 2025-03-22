using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Links;

public record LinkStoppingEvent(ILink Link) : IEvent;