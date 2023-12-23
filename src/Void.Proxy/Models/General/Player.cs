using Void.Proxy.API;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Models.General;

public class Player(Link link) : IPlayer
{
    public ILink Link { get; } = link;
    public string? Brand { get; protected set; }
    public ClientType ClientType { get; protected set; }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }

    public void SetClientType(ClientType clientType)
    {
        ClientType = clientType;
    }

    public override string ToString() => Link.PlayerRemoteEndPoint?.ToString() ?? "Disposed?";
}
