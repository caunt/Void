---
title: Transient Service
description: Learn how to use transient services.
sidebar:
  order: 3
---

Transient services are a type of service that is instantiated every time it is requested.
This means that a new instance of the service is created each time it is injected.
Transient services are useful for lightweight, stateless services that do not need to maintain any shared state or resources.

## Example Definition
```csharp
public class MyTransientService
{
    public string SomeValue { get; set; } = "Value";
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
            // Transient services are instantiated every time they are requested
            services.AddTransient<MyTransientService>();
        });
    }
}
```

## Example Injection
```csharp
public class MySingletonService(MyTransientService transientService)
{
    public void DoSomething()
    {
        // Access your transient service instance
        Console.WriteLine(transientService.SomeValue);
    }
}
```

## Example Manual Injection
```csharp
public class MySingletonService(IServiceProvider services)
{
    public void DoSomething()
    {
        // Access your transient service instance
        var transientService = services.GetRequiredService<MyTransientService>();
        Console.WriteLine(transientService.SomeValue);

        // Each time you call GetRequiredService, a new instance of MyTransientService is created
    }
}
```
