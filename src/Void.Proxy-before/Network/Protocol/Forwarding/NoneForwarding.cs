﻿namespace Void.Proxy.Network.Protocol.Forwarding;

public class NoneForwarding : IForwarding
{
    public ForwardingMode Mode => ForwardingMode.None;
}