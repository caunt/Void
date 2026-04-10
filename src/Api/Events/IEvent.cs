using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events;

public interface IEvent;

public interface IScopedEvent : IEvent
{
    public IPlayer Player { get; }
}

/// <summary>
/// Represents an event whose listeners can communicate an outcome by setting <see cref="Result" />.
/// </summary>
/// <typeparam name="T">The type of value produced by listeners for this event.</typeparam>
/// <remarks>
/// The event service publishes the event to listeners first and then returns the final <see cref="Result" /> value to the caller.
/// </remarks>
public interface IEventWithResult<T> : IEvent
{
    /// <summary>
    /// Gets or sets the value produced while handling the event.
    /// </summary>
    /// <value>
    /// The current event result, or <see langword="null" /> when no listener assigned a value.
    /// </value>
    public T? Result { get; set; }
}

public interface IScopedEventWithResult<T> : IScopedEvent, IEventWithResult<T>;
