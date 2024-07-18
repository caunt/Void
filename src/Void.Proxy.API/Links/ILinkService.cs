using Void.Proxy.API.Players;

namespace Void.Proxy.API.Links;

public interface ILinkService
{
    public ValueTask ConnectPlayerAnywhereAsync(IPlayer player);
}