﻿namespace Void.Proxy.API.Events;

public interface IEvent
{
}

public interface IEventWithResult<T> : IEvent
{
    public T? Result { get; set; }
}