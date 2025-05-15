---
title: Listening to Events
description: Learn how to listen to events in your plugin.
sidebar:
  order: 1
---

Events are a great way to communicate between different plugins and proxy. 
They allow you to respond to specific actions or changes in the game, such as a player joining or leaving.

## Subscribing to events
You can subscribe to events by applying the `Subscribe` attribute to a method in your class that inherits `IEventListener` interface.
```csharp
public class MyPlugin : IPlugin
{
    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        // Handle the event here
    }
}
```

:::tip
The `IPlugin` interface inherits `IEventListener` itself, so you can subscribe to events directly in your plugin class.
However, in most cases, you must apply `IEventListener` interface to your classes.
:::

## Async Events
Most of the time, you will want to interact with players when receiving an event.
Since the proxy is just network IO tool - such interactions are always implemented asynchronously, so you will need to make your listener asynchronous as well.
Your method can inject `CancellationToken` for graceful player network lifetime handling by proxy.

You do not need to think or worry about lifetime of cancellation tokens, just pass them down to `async` methods you call.
```csharp
[Subscribe]
public async ValueTask OnPlayerConnected(PlayerConnectedEvent @event, CancellationToken cancellationToken)
{
    // It is highly recommended to pass cancellation token in all async methods you call.
    // This helps proxy to correctly manage player network lifetime.
    await @event.Player.KickAsync("You are not allowed to join this server.", cancellationToken);
}
```

## Listening to events
For your managed services include `IEventListener` interface on your service class.
```csharp
public class MySingletonService : IEventListener
{
    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
    }
}
```

## Listening to events in Scoped services
Just like with other services, you should apply `IEventListener` interface to your scoped service class.
However, listening to events in scoped services is a bit different.
Scoped events are filtered by the player context, so you will only receive events that are relevant to the player that owns the service.
All other types of events will be not be filtered, since they are not scoped.
Scoped events filter can be disabled by applying `bypassScopedFilter: true` to the `Subscribe` attribute.
```csharp
public class MyScopedService(IPlayerContext context, ILogger logger) : IEventListener
{
    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        // Code here will be executed only when @event.Player and context.Player are same player.
        logger.LogInformation("Player {Player} is in its own context? {Result}", @event.Player.Name, @event.Player == context.Player);
    }

    [Subscribe(bypassScopedFilter: true)]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        // Code here will be executed for all players in all scopes.
    }
}
```
