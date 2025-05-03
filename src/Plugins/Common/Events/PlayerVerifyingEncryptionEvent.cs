using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Events;

public record PlayerVerifyingEncryptionEvent(IPlayer Player, ILink Link) : IScopedEventWithResult<bool>
{
    public bool Result { get; set; }
}
