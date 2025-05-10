---
title: Creating a Service
description: Learn how to use services in your plugin
---

Services are used to build a Dependency Injection (DI) container.
Your services will be shared across all plugins, and you can inject other plugins services into your plugin, if needed.

To learn more about DI, see [**Microsoft DI Documentation**](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).

## Registration

Inject `DependencyService` into your main plugin constructor:
```csharp
public class MyPlugin(IDependencyService dependencies) : IPlugin
{

}
```
Register your services with injected `DependencyService`:
```csharp
dependencies.Register(services =>
{
    services.AddSingleton<ExampleSingletonService>();
    services.AddScoped<ExampleScopedService>();
    services.AddTransient<ExampleTransientService>();
});
```
Your services will be automatically activated (instantiated) and subscribed to events, if not delayed explicitly.
You can delay your services activation, by providing `activate: false` parameter to the registration method:
```csharp
dependencies.Register(services =>
{
    services.AddSingleton<ExampleSingletonService>();
    services.AddScoped<ExampleScopedService>();
    services.AddTransient<ExampleTransientService>();
}, activate: false);
```

## Injection
You can inject your plugin or any other plugins services that are registered in the DI container into your classes.
```csharp
public class ExampleSingletonService(ExampleTransientService transientService)
{
}
```
```csharp
public class ExampleScopedService(ExampleSingletonService singletonService)
{
}
```

Proxy has a predefined set of services that are registered by default.
They are called API services in this documentation book.
```csharp
public class ExampleScopedService(
    ICommandService commands, 
    IConfigurationService configurations,
    ICryptoService crypto,
    IEventService events,
    IPlayerService players, 
    IServerService servers, 
    ILinkService links)
{
    
}
```

## Lifetimes
Services in the DI container can have different lifetimes, which determine how long they persist and how often they are created. These lifetimes are:

1. [**Singleton**](../types/singleton): A single instance is created and shared throughout the application.
2. [**Scoped**](../types/scoped): An instance is created once per player.
3. [**Transient**](../types/transient): A new instance is provided every time it is requested.

See the detailed description of each lifetime in the [**Microsoft Documentation**](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes).

## Example
See the [**ExamplePlugin.cs**](https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/ExamplePlugin.cs) for services registrations and [**Services**](https://github.com/caunt/Void/tree/main/src/Plugins/ExamplePlugin/Services) directory for services usage and implementations.
