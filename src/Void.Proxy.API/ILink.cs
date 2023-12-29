using System.Net;
using Void.Proxy.API.Network.IO;

namespace Void.Proxy.API;

public interface ILink : IDisposable
{
    public IPlayer Player { get; }
    public IServer? Server { get; }
    public ServerInfo? ServerInfo { get; }
    public EndPoint? PlayerRemoteEndPoint { get; }
    public EndPoint? ServerRemoteEndPoint { get; }

    public void Connect(ServerInfo serverInfo, IMinecraftChannel minecraftChannel);
    public void StartForwarding();
}
