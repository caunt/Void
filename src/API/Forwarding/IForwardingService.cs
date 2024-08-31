namespace Void.Proxy.API.Forwarding;

public interface IForwardingService
{
    IReadOnlyList<IForwarding> All { get; }

    void Register(IForwarding forwarding);
    void RegisterDefault();
}