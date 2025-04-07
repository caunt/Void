using Void.Common.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Events.Links;

public record LinkStoppingEvent(ILink Link) : IEvent;
