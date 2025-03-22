using Void.Proxy.API.Players;

namespace Void.Proxy.API.Chat.Commands;

public interface IPlayerCommandSource
{
    public IPlayer Player { get; set; }
}
