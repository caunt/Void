# Scoped

Scoped services are a type of service that is instantiated once per player session.
This means that a new instance of the service is created for each player, and it is shared across all components that depend on it within that player's context.
Scoped services are useful for managing player-specific state or resources that should not be shared across different players.

## Example Definition
IPlayerContext may be injected into your service to access the player context.
You can get player instance, other player-scoped services, or network channel from it.

```csharp
public class MyScopedService(IPlayerContext context)
{
}
```

## Example Registration
```csharp
public class MyPlugin(DependencyService services) : IPlugin
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
