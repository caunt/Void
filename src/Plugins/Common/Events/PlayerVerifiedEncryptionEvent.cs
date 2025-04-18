using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.Common.Events;

public record PlayerVerifiedEncryptionEvent(ILink Link) : IEvent;
