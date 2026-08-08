
namespace Void.Proxy.Api.Events.Services;

/// <summary>
/// Publishes proxy events and manages objects that contain subscribed event handlers.
/// </summary>
/// <remarks>
/// Handlers run in <see cref="PostOrder" /> order. Player-scoped events are filtered against scoped listeners unless the handler's <see cref="SubscribeAttribute.BypassScopedFilter" /> value is enabled.
/// </remarks>
public interface IEventService
{
    /// <summary>
    /// Gets a snapshot of listener entries currently registered with the service.
    /// </summary>
    /// <value>The registered listeners. A listener can appear more than once when it contains multiple subscribed methods.</value>
    public IEnumerable<IEventListener> Listeners { get; }

    /// <summary>
    /// Constructs and publishes an event.
    /// </summary>
    /// <typeparam name="T">The event type to construct and publish.</typeparam>
    /// <param name="cancellationToken">A token linked to the token passed to asynchronous handlers.</param>
    /// <returns>A task that completes after eligible handlers have run.</returns>
    public ValueTask ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new();

    /// <summary>
    /// Publishes an existing event instance.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">A token linked to the token passed to asynchronous handlers.</param>
    /// <returns>A task that completes after eligible handlers have run.</returns>
    public ValueTask ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;

    /// <summary>
    /// Publishes an event and returns the result left by its handlers.
    /// </summary>
    /// <typeparam name="TResult">The event result type.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">A token linked to the token passed to asynchronous handlers.</param>
    /// <returns>The final value of the event's result property. For nullable result types, this can be <see langword="null" /> when no handler assigns a value.</returns>
    public ValueTask<TResult?> ThrowWithResultAsync<TResult>(IEventWithResult<TResult> @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Constructs and publishes a result-bearing event, then returns its result.
    /// </summary>
    /// <typeparam name="T">The event type to construct and publish.</typeparam>
    /// <typeparam name="TResult">The event result type.</typeparam>
    /// <param name="cancellationToken">A token linked to the token passed to asynchronous handlers.</param>
    /// <returns>The final value of the event's result property. For nullable result types, this can be <see langword="null" /> when no handler assigns a value.</returns>
    public ValueTask<TResult?> ThrowWithResultAsync<T, TResult>(CancellationToken cancellationToken = default) where T : IEventWithResult<TResult?>, new();

    /// <summary>
    /// Waits until a subsequently published event satisfies a predicate.
    /// </summary>
    /// <param name="condition">The predicate evaluated for each published event.</param>
    /// <param name="cancellationToken">A token used to cancel and remove the pending waiter.</param>
    /// <returns>A task that completes when <paramref name="condition" /> returns <see langword="true" />.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken" /> is canceled before a matching event is published.</exception>
    public ValueTask WaitAsync(Func<IEvent, bool> condition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers subscribed methods from a sequence of listener objects.
    /// </summary>
    /// <param name="listeners">The listeners to inspect and register.</param>
    /// <param name="cancellationToken">A token linked to handler invocations for these registrations.</param>
    public void RegisterListeners(IEnumerable<IEventListener> listeners, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers subscribed methods from listener objects with an invocation cancellation token.
    /// </summary>
    /// <param name="cancellationToken">A token linked to handler invocations for these registrations.</param>
    /// <param name="listeners">The listeners to inspect and register.</param>
    public void RegisterListeners(CancellationToken cancellationToken = default, params IEventListener[] listeners);

    /// <summary>
    /// Registers subscribed methods from listener objects without a registration cancellation token.
    /// </summary>
    /// <param name="listeners">The listeners to inspect and register.</param>
    public void RegisterListeners(params IEventListener[] listeners);

    /// <summary>
    /// Removes all subscribed methods belonging to a sequence of listener objects.
    /// </summary>
    /// <param name="listeners">The listeners to unregister.</param>
    public void UnregisterListeners(IEnumerable<IEventListener> listeners);

    /// <summary>
    /// Removes all subscribed methods belonging to listener objects.
    /// </summary>
    /// <param name="listeners">The listeners to unregister.</param>
    public void UnregisterListeners(params IEventListener[] listeners);
}
