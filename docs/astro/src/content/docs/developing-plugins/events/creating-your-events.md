---
title: Creating your Events
description: Learn how to create your own events in your plugin
sidebar:
  order: 2
---

Many events are implemented by the proxy itself, but you can make your own events by implementing the `IEvent` interface or one of its derived interfaces.

## Simple Event
Simple events are just a wrapper around the event data. They are used to pass data between different parts of the code. 
You can create a simple event by implementing the `IEvent` interface and adding properties to it.
```csharp
public record MyEvent(string Value1, int Value2, byte[] Value3) : IEvent;
```

## Event with Result
Event with result is a special type of event that allows you to return a result from the event handlers.
If multiple handlers set the result, you will get the last one.
```csharp
public record MyEvent(string SomeValue) : IEventWithResult<int>
{
    public int? Result { get; set; }
}
```

## Cancellable Event
You can define your events as cancellable by making Result type boolean.
```csharp
public record MyEvent(string SomeValue) : IEventWithResult<bool>
{
    public bool? Result { get; set; }

    public void Cancel() => Result = true;
}
```

## Scoped Event
Your event can be scoped to a specific player. You can do this by implementing the `IScopedEvent` interface.
With this interface you are required to specify the Player to which this event is scoped.
This event will be filtered across scoped listeners and passed to all non-scoped listeners.
Howerer, scoped listeners can still receive that event out of their scope, by applying `bypassScopedFilter: true` to the `Subscribe` attribute.
```csharp
public record MyEvent(IPlayer Player, string SomeValue) : IScopedEvent;
```

## Scoped Event with Result
You can also create a scoped event with result.
```csharp
public record MyEvent(IPlayer Player, string SomeValue) : IScopedEventWithResult<int>
{
    public int? Result { get; set; }
}
```

## Throwing your Events
To throw your event, inject `IEventService` into your class.

```csharp
public class MyService(IEventService events) : IEventListener
{
    [Subscribe]
    public async ValueTask OnPlayerConnected(PlayerConnectedEvent @event, CancellationToken cancellationToken)
    {
        // Event without result
        await events.ThrowAsync(new MyEvent(@event.Player, "Hello world!"), cancellationToken);
        
        // Event with result
        var result = await events.ThrowWithResultAsync(new MyEvent(@event.Player, "Hello world!"), cancellationToken);
    }
}
```