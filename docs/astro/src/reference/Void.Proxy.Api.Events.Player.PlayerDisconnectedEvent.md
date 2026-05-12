# <a id="Void_Proxy_Api_Events_Player_PlayerDisconnectedEvent"></a> Class PlayerDisconnectedEvent

Namespace: [Void.Proxy.Api.Events.Player](Void.Proxy.Api.Events.Player.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record PlayerDisconnectedEvent : IScopedEvent, IEvent, IEquatable<PlayerDisconnectedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerDisconnectedEvent](Void.Proxy.Api.Events.Player.PlayerDisconnectedEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PlayerDisconnectedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Player_PlayerDisconnectedEvent__ctor_Void_Proxy_Api_Players_IPlayer_"></a> PlayerDisconnectedEvent\(IPlayer\)

```csharp
public PlayerDisconnectedEvent(IPlayer Player)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

## Properties

### <a id="Void_Proxy_Api_Events_Player_PlayerDisconnectedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

