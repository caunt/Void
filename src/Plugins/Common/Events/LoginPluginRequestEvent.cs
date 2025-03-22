using Void.Proxy.API.Events;
using Void.Proxy.API.Links;

namespace Void.Proxy.Plugins.Common.Events;

public record LoginPluginRequestEvent(ILink Link, string Channel, byte[] Data) : IEventWithResult<byte[]>
{
    public byte[]? Result { get; set; }
}