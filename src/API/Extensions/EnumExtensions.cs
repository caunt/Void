using Void.Proxy.API.Links;
using Void.Proxy.API.Network;

namespace Void.Proxy.API.Extensions;

public static class EnumExtensions
{
    public static object? FromLink(this Side side, ILink link)
    {
        return side switch
        {
            Side.Client => link.Player,
            Side.Server => link.Server,
            Side.Proxy => typeof(IProxy),
            _ => null
        };
    }
}