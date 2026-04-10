# <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyResolver"></a> Interface IDependencyResolver

Namespace: [Void.Proxy.Api.Plugins.Dependencies](Void.Proxy.Api.Plugins.Dependencies.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IDependencyResolver
```

## Methods

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyResolver_Resolve_System_Runtime_Loader_AssemblyLoadContext_System_Reflection_AssemblyName_"></a> Resolve\(AssemblyLoadContext, AssemblyName\)

```csharp
Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
```

#### Parameters

`context` [AssemblyLoadContext](https://learn.microsoft.com/dotnet/api/system.runtime.loader.assemblyloadcontext)

`assemblyName` [AssemblyName](https://learn.microsoft.com/dotnet/api/system.reflection.assemblyname)

#### Returns

 [Assembly](https://learn.microsoft.com/dotnet/api/system.reflection.assembly)?

