namespace Void.Proxy.Api.Servers;

public interface IServerService
{
    public IEnumerable<IServer> All { get; }
}
