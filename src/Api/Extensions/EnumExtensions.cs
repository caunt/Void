using Void.Common.Network;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Extensions;

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
