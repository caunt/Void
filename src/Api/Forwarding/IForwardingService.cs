namespace Void.Proxy.Api.Forwarding;

public interface IForwardingService
{
    IReadOnlyList<IForwarding> All { get; }

    void Register(IForwarding forwarding);
    void RegisterDefault();
}
