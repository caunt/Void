---
title: Singleton Service
description: Learn how to use singleton services.
sidebar:
  order: 1
---

Singleton services are a type of service that is instantiated once and shared across the entire application. 
This means that there is only one instance of the service, and it is used by all components that depend on it. 
Singleton services are useful for managing shared resources or state that should be consistent across the application.

## Example Definition
```csharp
public class MySingletonService
{
    public string SomeValue { get; set; } = "Value";
}
```

## Example Registration
```csharp
public class MyPlugin(IDependencyService dependencies) : IPlugin
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
            // Singleton services are instantiated once and shared across the entire application
            services.AddSingleton<MySingletonService>();
        });
    }
}
```

## Example Injection
```csharp
public class MyAnotherSingletonService(MySingletonService singletonService)
{
    public void DoSomething()
    {
        // Access your singleton service instance
        Console.WriteLine(singletonService.SomeValue);
    }
}
```