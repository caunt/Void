using Void.Proxy.Api.Forwarding;

namespace Void.Proxy.Forwarding;

public class ForwardingService : IForwardingService
{
    private readonly List<IForwarding> _registered = [];

    public IReadOnlyList<IForwarding> All => _registered;

    public void Register(IForwarding forwarding)
    {
        _registered.Add(forwarding);
    }

    public void RegisterDefault()
    {
        Register(new NoneForwarding());
    }
}
