namespace Void.Proxy.Network.Protocol.Forwarding;

public class AutoForwarding : IForwarding
{
    public ForwardingMode Mode => ForwardingMode.Auto;
}