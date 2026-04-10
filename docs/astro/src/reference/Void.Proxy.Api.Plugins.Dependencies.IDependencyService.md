# <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService"></a> Interface IDependencyService

Namespace: [Void.Proxy.Api.Plugins.Dependencies](Void.Proxy.Api.Plugins.Dependencies.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IDependencyService : IEventListener, IServiceProvider
```

#### Implements

[IEventListener](Void.Proxy.Api.Events.IEventListener.md), 
[IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider)

## Methods

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_ActivatePlayerContext_Void_Proxy_Api_Players_Contexts_IPlayerContext_"></a> ActivatePlayerContext\(IPlayerContext\)

```csharp
void ActivatePlayerContext(IPlayerContext context)
```

#### Parameters

`context` [IPlayerContext](Void.Proxy.Api.Players.Contexts.IPlayerContext.md)

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_CreateInstance__1_System_Threading_CancellationToken_System_Object___"></a> CreateInstance<TService\>\(CancellationToken, params object\[\]\)

```csharp
TService CreateInstance<TService>(CancellationToken cancellationToken = default, params object[] parameters)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

`parameters` [object](https://learn.microsoft.com/dotnet/api/system.object)\[\]

#### Returns

 TService

#### Type Parameters

`TService` 

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_CreateInstance__1_System_Type_System_Threading_CancellationToken_System_Object___"></a> CreateInstance<TService\>\(Type, CancellationToken, params object\[\]\)

```csharp
TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters)
```

#### Parameters

`serviceType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

`parameters` [object](https://learn.microsoft.com/dotnet/api/system.object)\[\]

#### Returns

 TService

#### Type Parameters

`TService` 

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_CreateInstance_System_Type_System_Threading_CancellationToken_System_Object___"></a> CreateInstance\(Type, CancellationToken, params object\[\]\)

```csharp
object CreateInstance(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters)
```

#### Parameters

`serviceType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

`parameters` [object](https://learn.microsoft.com/dotnet/api/system.object)\[\]

#### Returns

 [object](https://learn.microsoft.com/dotnet/api/system.object)

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_CreatePlayerComposite_Void_Proxy_Api_Players_IPlayer_"></a> CreatePlayerComposite\(IPlayer\)

```csharp
IServiceProvider CreatePlayerComposite(IPlayer player)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

#### Returns

 [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider)

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_DisposePlayerContext_Void_Proxy_Api_Players_Contexts_IPlayerContext_"></a> DisposePlayerContext\(IPlayerContext\)

```csharp
void DisposePlayerContext(IPlayerContext context)
```

#### Parameters

`context` [IPlayerContext](Void.Proxy.Api.Players.Contexts.IPlayerContext.md)

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_GetService__1"></a> GetService<TService\>\(\)

```csharp
TService? GetService<TService>()
```

#### Returns

 TService?

#### Type Parameters

`TService` 

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_Register_System_Action_Microsoft_Extensions_DependencyInjection_ServiceCollection__System_Boolean_"></a> Register\(Action<ServiceCollection\>, bool\)

```csharp
void Register(Action<ServiceCollection> configure, bool activate = true)
```

#### Parameters

`configure` [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[ServiceCollection](https://learn.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection)\>

`activate` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Plugins_Dependencies_IDependencyService_TryGetScopedPlayerContext_System_Object_Void_Proxy_Api_Players_Contexts_IPlayerContext__"></a> TryGetScopedPlayerContext\(object, out IPlayerContext\)

```csharp
bool TryGetScopedPlayerContext(object instance, out IPlayerContext context)
```

#### Parameters

`instance` [object](https://learn.microsoft.com/dotnet/api/system.object)

`context` [IPlayerContext](Void.Proxy.Api.Players.Contexts.IPlayerContext.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

