using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events;

/// <summary>
/// Identifies a value that can be published through the proxy event service.
/// </summary>
public interface IEvent;

/// <summary>
/// Represents an event associated with a specific player.
/// </summary>
/// <remarks>
/// Scoped event listeners are ordinarily filtered so that they receive events for the player scope in which they were registered.
/// </remarks>
public interface IScopedEvent : IEvent
{
    /// <summary>
    /// Gets the player whose scope contains the event.
    /// </summary>
    /// <value>The player associated with the event.</value>
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

/// <summary>
/// Represents a player-scoped event whose listeners can communicate an outcome.
/// </summary>
/// <typeparam name="T">The type of value produced by listeners for this event.</typeparam>
public interface IScopedEventWithResult<T> : IScopedEvent, IEventWithResult<T>;
