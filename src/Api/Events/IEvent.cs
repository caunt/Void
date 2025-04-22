using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events;

public interface IEvent;

public interface IScopedEvent : IEvent
{
    public IPlayer Player { get; }
}

public interface IEventWithResult<T> : IEvent
{
    public T? Result { get; set; }
}

public interface IScopedEventWithResult<T> : IScopedEvent, IEventWithResult<T>;
