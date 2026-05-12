# <a id="Void_Proxy_Api_Players_IPlayer"></a> Interface IPlayer

Namespace: [Void.Proxy.Api.Players](Void.Proxy.Api.Players.md)  
Assembly: Void.Proxy.Api.dll  

Represents a connected player and exposes information about their network
connection and execution context.

```csharp
public interface IPlayer : IEquatable<IPlayer>, ICommandSource, IAsyncDisposable, IDisposable
```

#### Implements

[IEquatable<IPlayer\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

## Properties

### <a id="Void_Proxy_Api_Players_IPlayer_Client"></a> Client

Gets the underlying <xref href="System.Net.Sockets.TcpClient" data-throw-if-not-resolved="false"></xref> used for network.
communication.

```csharp
TcpClient Client { get; }
```

#### Property Value

 [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

### <a id="Void_Proxy_Api_Players_IPlayer_ConnectedAt"></a> ConnectedAt

Gets the date and time when the player connected to the proxy.

```csharp
DateTimeOffset ConnectedAt { get; }
```

#### Property Value

 [DateTimeOffset](https://learn.microsoft.com/dotnet/api/system.datetimeoffset)

### <a id="Void_Proxy_Api_Players_IPlayer_Context"></a> Context

Gets the context containing services and state associated with the
player.

```csharp
IPlayerContext Context { get; }
```

#### Property Value

 [IPlayerContext](Void.Proxy.Api.Players.Contexts.IPlayerContext.md)

### <a id="Void_Proxy_Api_Players_IPlayer_Name"></a> Name

Gets the name of the player. Falls back to IP address if unavailable.

```csharp
string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Proxy_Api_Players_IPlayer_RemoteEndPoint"></a> RemoteEndPoint

Gets the textual representation of the client's remote endpoint.

```csharp
string RemoteEndPoint { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Proxy_Api_Players_IPlayer_GetStableHashCode"></a> GetStableHashCode\(\)

Computes a stable hash code for the current instance. This is useful when the instance is upgraded or replaced to another implementation.

```csharp
int GetStableHashCode()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

An integer representing the hash code of the current instance, derived from the <xref href="Void.Proxy.Api.Players.IPlayer.Client" data-throw-if-not-resolved="false"></xref> property.

#### Remarks

The hash code is based on the <xref href="Void.Proxy.Api.Players.IPlayer.Client" data-throw-if-not-resolved="false"></xref> property.

