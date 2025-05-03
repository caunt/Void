---
title: Scoped Services
description: Learn how to use scoped services in your plugins
---

Scoped services are a type of service that is instantiated once per player session.
This means that a new instance of the service is created for each player, and it is shared across all components that depend on it within that player's context.
Scoped services are useful for managing player-specific state or resources that should not be shared across different players.

## Example Definition
`IPlayerContext` may be injected into your scoped service to access the player context.
You can get player instance, other player-scoped services, or network channel from it.

```csharp
public class MyScopedService(IPlayerContext context)
{
}
```

## Example Registration
```csharp
public class MyPlugin(IDependencyService services) : IPlugin
{
    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugins load events except ours
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
            // Scoped services are instantiated once per player session
            services.AddScoped<MyScopedService>();
        });
    }
}
```

## Example Injection
Scoped services can be injected into other scoped services or manually resolved from any player instance.

```csharp
public class MyAnotherScopedService(MyScopedService scopedService)
{
}
```

## Example Manual Injection
```csharp
public class MySingletonService(IPlayerService players)
{
    public void DoSomething()
    {
        foreach (var player in players.All)
        {
            // Access your scoped service instance for each player
            var scopedService = player.Context.Services.GetRequiredService<MyScopedService>();
        }
    }
}
```

## Events
Scoped services subscribed to events that implement `IScopedEvent` interface will be automatically filtered to the current player context.
For example, if you listen to `PlayerConnectedEvent`, the event will be automatically filtered to the current player only context.
```csharp
public class MyScopedService(IPlayerContext context) : IEventListener
{
    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        // This event will be only triggered for IPlayerContext.Player instance
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        // This event also will be only triggered for IPlayerContext.Player instance
    }
}
```

While events being filtered, scoped services are still instantiated for each player. 
So all players and their respective scoped services will be notified about the event.
This helps in player-specific resources isolation.

## Filtered Events usage example
```csharp
public class PlayerPositionService(IPlayerContext context) : IEventListener
{
    public decimal PositionX { get; private set; }
    public decimal PositionY { get; private set; }
    public decimal PositionZ { get; private set; }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        // This code is pure example.
        // If you need player positions, make sure to implement this packet yourself.

        if (@event.Message is not PlayerPositionPacket packet)
            return;

        PositionX = packet.X;
        PositionY = packet.Y;
        PositionZ = packet.Z;
    }
}
```

Now all players have their own instance of `PlayerPositionService` and each one contains current player position.
You can get this service from player directly to access actual player position.
```csharp
public class TrackerService(IPlayerService players)
{
    public void DoSomething()
    {
        foreach (var player in players.All)
        {
            var scopedService = player.Context.Services.GetRequiredService<PlayerPositionService>();

            var positionX = scopedService.PositionX;
            var positionY = scopedService.PositionY;
            var positionZ = scopedService.PositionZ;

            // Do whatever you want with player position
        }
    }
}
```

Most of the events that have Player property are already implemented as `IScopedEvent`.
While you can listen to them in Scoped services, they are still available for Singleton services.
In Singleton context, you will receive events **not filtered**. Meaning you will receive events for all players in single service.

If you would like to not filter events in scoped service, pass `bypassScopedFilter: true` to the `Subscribe` attribute.
```csharp
public class MyScopedService(IPlayerContext context) : IEventListener
{
    [Subscribe(bypassScopedFilter: true)]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        // This event will be triggered for all players
        // regardless of the current player context
    }
}
```
