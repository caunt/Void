using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.Common.Events;

public record PlayerVerifyingEncryptionEvent(ILink Link) : IEventWithResult<bool>
{
    public bool Result { get; set; }
}
