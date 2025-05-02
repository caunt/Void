---
title: Events
description: Learn how to use events in your plugins
---

Events are a great way to communicate between different plugins and proxy. 
They allow you to respond to specific actions or changes in the game, such as a player joining or leaving.

## Subscribing to events
You can subscribe to events by applying the `Subscribe` attribute to a method in your class.
Many events are implmented by the proxy itself, and you can make your own events by implementing the `IEvent` interface.
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

## Async Events
Most of the time, you will want to interact with players when receiving an event.
Since the proxy is just network IO tool - such interactions are always implemented asynchronously, so you will need to make your listener asynchronous as well.
Your method can inject `CancellationToken` for graceful player network lifetime.

You do not need to think or worry about lifetime of cancellation tokens, just pass them to `async` methods you call.
```csharp
[Subscribe]
public async ValueTask OnPlayerConnected(PlayerConnectedEvent @event, CancellationToken cancellationToken)
{
    // It is highly recommended to pass cancellation token in all async methods you call.
    await @event.Player.KickAsync("You are not allowed to join this server.", cancellationToken);
}
```

## Listening to events
All `IPlugin` implementations are automatically scanned for the `Subscribe` attribute.
For your managed services please include `IEventListener` interface on your service class.
```csharp
public class MySingletonService : IEventListener
{
    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
    }
}

public class MyScopedService(IPlayerContext context) : IEventListener
{
    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
    }
}
```

## Registering services with events
```csharp
public class MyPlugin(DependencyService services) : IPlugin
{
    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
            services.AddSingleton<MySingletonService>();
            services.AddScoped<MyScopedService>();
        });
    }
}
```
