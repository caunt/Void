using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Events;

public record LoginPluginRequestEvent(IPlayer Player, ILink Link, string Channel, byte[] Data) : IScopedEventWithResult<byte[]>
{
    public byte[]? Result { get; set; }
}
