# <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartingEvent"></a> Class AuthenticationStartingEvent

Namespace: [Void.Proxy.Api.Events.Authentication](Void.Proxy.Api.Events.Authentication.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record AuthenticationStartingEvent : IScopedEventWithResult<AuthenticationSide>, IScopedEvent, IEventWithResult<AuthenticationSide>, IEvent, IEquatable<AuthenticationStartingEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AuthenticationStartingEvent](Void.Proxy.Api.Events.Authentication.AuthenticationStartingEvent.md)

#### Implements

[IScopedEventWithResult<AuthenticationSide\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<AuthenticationSide\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<AuthenticationStartingEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartingEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_"></a> AuthenticationStartingEvent\(ILink, IPlayer\)

```csharp
public AuthenticationStartingEvent(ILink Link, IPlayer Player)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

## Properties

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartingEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartingEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartingEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public AuthenticationSide Result { get; set; }
```

#### Property Value

 [AuthenticationSide](Void.Proxy.Api.Events.Authentication.AuthenticationSide.md)

