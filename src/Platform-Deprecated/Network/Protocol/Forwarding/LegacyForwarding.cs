namespace Void.Proxy.Network.Protocol.Forwarding;

public class LegacyForwarding : IForwarding
{
    public bool IncludeAddress { get; set; }
    public bool IncludeUuid { get; set; }
    public bool IncludeSkin { get; set; }
    public ForwardingMode Mode => ForwardingMode.Legacy;
}