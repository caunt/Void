# <a id="Void_Proxy_Api_Events_Player_PlayerSearchServerEvent"></a> Class PlayerSearchServerEvent

Namespace: [Void.Proxy.Api.Events.Player](Void.Proxy.Api.Events.Player.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record PlayerSearchServerEvent : IScopedEventWithResult<IServer>, IScopedEvent, IEventWithResult<IServer>, IEvent, IEquatable<PlayerSearchServerEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerSearchServerEvent](Void.Proxy.Api.Events.Player.PlayerSearchServerEvent.md)

#### Implements

[IScopedEventWithResult<IServer\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<IServer\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PlayerSearchServerEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Player_PlayerSearchServerEvent__ctor_Void_Proxy_Api_Players_IPlayer_"></a> PlayerSearchServerEvent\(IPlayer\)

```csharp
public PlayerSearchServerEvent(IPlayer Player)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

## Properties

### <a id="Void_Proxy_Api_Events_Player_PlayerSearchServerEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Player_PlayerSearchServerEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public IServer? Result { get; set; }
```

#### Property Value

 [IServer](Void.Proxy.Api.Servers.IServer.md)?

