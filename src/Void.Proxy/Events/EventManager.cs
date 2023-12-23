using Void.Proxy.API.Events;

namespace Void.Proxy.Events;

public class EventManager : IEventManager
{
    public async Task ThrowAsync<T>() where T : IEvent
    {

    }
}
