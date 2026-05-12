# <a id="Void_Proxy_Api_Events_Plugins_PluginUnloadedEvent"></a> Class PluginUnloadedEvent

Namespace: [Void.Proxy.Api.Events.Plugins](Void.Proxy.Api.Events.Plugins.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record PluginUnloadedEvent : IEvent, IEquatable<PluginUnloadedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PluginUnloadedEvent](Void.Proxy.Api.Events.Plugins.PluginUnloadedEvent.md)

#### Implements

[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PluginUnloadedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Plugins_PluginUnloadedEvent__ctor_Void_Proxy_Api_Plugins_IPlugin_"></a> PluginUnloadedEvent\(IPlugin\)

```csharp
public PluginUnloadedEvent(IPlugin Plugin)
```

#### Parameters

`Plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

## Properties

### <a id="Void_Proxy_Api_Events_Plugins_PluginUnloadedEvent_Plugin"></a> Plugin

```csharp
public IPlugin Plugin { get; init; }
```

#### Property Value

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

