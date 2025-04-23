using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Players.Contexts;

public class PlayerContextAccessor : IPlayerContextAccessor
{
    public IPlayerContext? Context { get; set; }
}
