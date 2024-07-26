﻿namespace Void.Proxy.API.Events.Services;

public interface IEventService
{
    public ValueTask ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new();
    public ValueTask ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
    public void RegisterListeners(params IEventListener[] listeners);
    public void UnregisterListeners(params IEventListener[] listeners);
}