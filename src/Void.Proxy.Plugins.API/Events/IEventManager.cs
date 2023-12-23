namespace Void.Proxy.API.Events;

public interface IEventManager
{
    public Task ThrowAsync<T>() where T : IEvent;
}
