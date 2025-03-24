using Void.Proxy.Api.Forwarding;

namespace Void.Proxy.Forwarding;

public class NoneForwarding : IForwarding
{
    public string Name => "none";
}