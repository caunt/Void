using System.Net;

namespace Void.Proxy.API;

public interface ILink : IDisposable
{
    public IPlayer Player { get; }
    public IServer? Server { get; }
    public ServerInfo? ServerInfo { get; }
    public EndPoint? PlayerRemoteEndPoint { get; }
    public EndPoint? ServerRemoteEndPoint { get; }

    public void Connect(ServerInfo serverInfo);
    public void StartForwarding();
}
