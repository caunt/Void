using Void.Proxy.Api.Players;

namespace Void.Proxy.Players.Extensions;

public static class PlayerExtensions
{
    public static IPlayer Unwrap(this IPlayer player)
    {
        return player switch
        {
            PlayerProxy proxy => Unwrap(proxy.Source),
            _ => player
        };
    }
}
