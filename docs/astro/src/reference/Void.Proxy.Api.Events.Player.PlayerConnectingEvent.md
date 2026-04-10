# <a id="Void_Proxy_Api_Events_Player_PlayerConnectingEvent"></a> Class PlayerConnectingEvent

Namespace: [Void.Proxy.Api.Events.Player](Void.Proxy.Api.Events.Player.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record PlayerConnectingEvent : IEventWithResult<IPlayer>, IEvent, IEquatable<PlayerConnectingEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerConnectingEvent](Void.Proxy.Api.Events.Player.PlayerConnectingEvent.md)

#### Implements

[IEventWithResult<IPlayer\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PlayerConnectingEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Player_PlayerConnectingEvent__ctor_System_Net_Sockets_TcpClient_System_Func_Void_Proxy_Api_Players_IPlayer_System_IServiceProvider__"></a> PlayerConnectingEvent\(TcpClient, Func<IPlayer, IServiceProvider\>\)

```csharp
public PlayerConnectingEvent(TcpClient Client, Func<IPlayer, IServiceProvider> GetServices)
```

#### Parameters

`Client` [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

`GetServices` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[IPlayer](Void.Proxy.Api.Players.IPlayer.md), [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider)\>

## Properties

### <a id="Void_Proxy_Api_Events_Player_PlayerConnectingEvent_Client"></a> Client

```csharp
public TcpClient Client { get; init; }
```

#### Property Value

 [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

### <a id="Void_Proxy_Api_Events_Player_PlayerConnectingEvent_GetServices"></a> GetServices

```csharp
public Func<IPlayer, IServiceProvider> GetServices { get; init; }
```

#### Property Value

 [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[IPlayer](Void.Proxy.Api.Players.IPlayer.md), [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider)\>

### <a id="Void_Proxy_Api_Events_Player_PlayerConnectingEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public IPlayer? Result { get; set; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)?

