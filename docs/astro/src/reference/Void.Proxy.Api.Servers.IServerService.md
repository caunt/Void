# <a id="Void_Proxy_Api_Servers_IServerService"></a> Interface IServerService

Namespace: [Void.Proxy.Api.Servers](Void.Proxy.Api.Servers.md)  
Assembly: Void.Proxy.Api.dll  

Provides access to the collection of servers available to the proxy.

```csharp
public interface IServerService
```

## Properties

### <a id="Void_Proxy_Api_Servers_IServerService_All"></a> All

Gets all configured servers.

```csharp
IEnumerable<IServer> All { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IServer](Void.Proxy.Api.Servers.IServer.md)\>

## Methods

### <a id="Void_Proxy_Api_Servers_IServerService_GetByName_System_String_"></a> GetByName\(string\)

```csharp
IServer? GetByName(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [IServer](Void.Proxy.Api.Servers.IServer.md)?

### <a id="Void_Proxy_Api_Servers_IServerService_TryGetByName_System_String_Void_Proxy_Api_Servers_IServer__"></a> TryGetByName\(string, out IServer\)

```csharp
bool TryGetByName(string name, out IServer server)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`server` [IServer](Void.Proxy.Api.Servers.IServer.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

