using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API;

public interface IPlayer
{
    public ILink Link { get; }
    public string? Brand { get; }
    public ClientType ClientType { get; }

    public void SetBrand(string brand);
    public void SetClientType(ClientType clientType);
}
