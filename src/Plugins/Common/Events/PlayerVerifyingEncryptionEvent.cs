using Void.Proxy.API.Events;
using Void.Proxy.API.Links;

namespace Void.Proxy.Plugins.Common.Events;

public record PlayerVerifyingEncryptionEvent(ILink Link) : IEventWithResult<bool>
{
    public bool Result { get; set; }
}
