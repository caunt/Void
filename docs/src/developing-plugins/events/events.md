# Events

Events are a great way to communicate between different plugins and proxy. 
They allow you to respond to specific actions or changes in the game, such as a player joining or leaving.

## Listening to events
All `IPlugin` implementations are subscribed to the events automatically.
For your managed services, use the `Subscribe` attribute to listen to the events and include `IEventListener` interface on your service class.
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

