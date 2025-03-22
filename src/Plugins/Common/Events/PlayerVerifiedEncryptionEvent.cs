using Void.Proxy.API.Events;
using Void.Proxy.API.Links;

namespace Void.Proxy.Plugins.Common.Events;

public record PlayerVerifiedEncryptionEvent(ILink Link) : IEvent;
