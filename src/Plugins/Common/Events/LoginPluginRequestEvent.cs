using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.Common.Events;

public record LoginPluginRequestEvent(ILink Link, string Channel, byte[] Data) : IEventWithResult<byte[]>
{
    public byte[]? Result { get; set; }
}
