# <a id="Void_Proxy_Api_Players_Contexts_IPlayerContext"></a> Interface IPlayerContext

Namespace: [Void.Proxy.Api.Players.Contexts](Void.Proxy.Api.Players.Contexts.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IPlayerContext : IDisposable, IAsyncDisposable
```

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Properties

### <a id="Void_Proxy_Api_Players_Contexts_IPlayerContext_Channel"></a> Channel

Gets or sets the network channel used for communication.

```csharp
INetworkChannel? Channel { get; set; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)?

### <a id="Void_Proxy_Api_Players_Contexts_IPlayerContext_IsDisposed"></a> IsDisposed

Gets the current state of the player context.

```csharp
bool IsDisposed { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Players_Contexts_IPlayerContext_Logger"></a> Logger

Gets the logger instance for logging player-specific events and information.

```csharp
ILogger Logger { get; }
```

#### Property Value

 [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

### <a id="Void_Proxy_Api_Players_Contexts_IPlayerContext_Player"></a> Player

Gets the current player instance.

```csharp
IPlayer Player { get; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Players_Contexts_IPlayerContext_Services"></a> Services

Gets the service provider that provides access to player scoped services.

```csharp
IServiceProvider Services { get; }
```

#### Property Value

 [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider)

