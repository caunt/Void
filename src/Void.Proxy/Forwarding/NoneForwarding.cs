using Void.Proxy.API.Forwarding;

namespace Void.Proxy.Forwarding;

public class NoneForwarding : IForwarding
{
    public string Name => "none";
}