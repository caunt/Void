namespace Void.Proxy.Api.Players.Contexts;

public interface IPlayerContextAccessor
{
    public IPlayerContext? Context { get; set; }
}
