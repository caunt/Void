namespace MinecraftProxy.Network.Protocol.Forwarding;

public class LegacyForwarding : IForwarding
{
    public ForwardingMode Mode => ForwardingMode.Legacy;
}