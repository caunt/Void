namespace Void.Proxy.Api.Commands;

public interface ICommandDispatcher
{
    public void Add(ICommandNode node);
}
