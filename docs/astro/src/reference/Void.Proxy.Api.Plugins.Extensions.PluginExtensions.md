# <a id="Void_Proxy_Api_Plugins_Extensions_PluginExtensions"></a> Class PluginExtensions

Namespace: [Void.Proxy.Api.Plugins.Extensions](Void.Proxy.Api.Plugins.Extensions.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public static class PluginExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PluginExtensions](Void.Proxy.Api.Plugins.Extensions.PluginExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Proxy_Api_Plugins_Extensions_PluginExtensions_GetPluginFromType__1_Void_Proxy_Api_Plugins_IPluginService_"></a> GetPluginFromType<T\>\(IPluginService\)

```csharp
public static IPlugin GetPluginFromType<T>(this IPluginService plugins)
```

#### Parameters

`plugins` [IPluginService](Void.Proxy.Api.Plugins.IPluginService.md)

#### Returns

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Plugins_Extensions_PluginExtensions_GetPluginFromType_Void_Proxy_Api_Plugins_IPluginService_System_Type_"></a> GetPluginFromType\(IPluginService, Type\)

```csharp
public static IPlugin GetPluginFromType(this IPluginService plugins, Type type)
```

#### Parameters

`plugins` [IPluginService](Void.Proxy.Api.Plugins.IPluginService.md)

`type` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

### <a id="Void_Proxy_Api_Plugins_Extensions_PluginExtensions_TryGetPluginFromType_Void_Proxy_Api_Plugins_IPluginService_System_Type_Void_Proxy_Api_Plugins_IPlugin__"></a> TryGetPluginFromType\(IPluginService, Type, out IPlugin\)

```csharp
public static bool TryGetPluginFromType(this IPluginService plugins, Type type, out IPlugin plugin)
```

#### Parameters

`plugins` [IPluginService](Void.Proxy.Api.Plugins.IPluginService.md)

`type` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

