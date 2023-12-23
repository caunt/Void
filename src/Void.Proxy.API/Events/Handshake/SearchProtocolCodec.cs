namespace Void.Proxy.API.Events.Handshake;

public class SearchProtocolCodec : IEvent
{
    public required ILink Link { get; init; }
}
