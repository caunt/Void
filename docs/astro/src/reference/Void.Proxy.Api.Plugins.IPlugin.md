# <a id="Void_Proxy_Api_Plugins_IPlugin"></a> Interface IPlugin

Namespace: [Void.Proxy.Api.Plugins](Void.Proxy.Api.Plugins.md)  
Assembly: Void.Proxy.Api.dll  

Represents a proxy plugin. Plugins extend proxy functionality by subscribing to events
and are identified by a human-readable name.

```csharp
public interface IPlugin : IEventListener
```

#### Implements

[IEventListener](Void.Proxy.Api.Events.IEventListener.md)

## Properties

### <a id="Void_Proxy_Api_Plugins_IPlugin_Name"></a> Name

Gets the human-readable name of this plugin.

```csharp
string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

