# <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartedEvent"></a> Class AuthenticationStartedEvent

Namespace: [Void.Proxy.Api.Events.Authentication](Void.Proxy.Api.Events.Authentication.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record AuthenticationStartedEvent : IScopedEventWithResult<AuthenticationResult>, IScopedEvent, IEventWithResult<AuthenticationResult>, IEvent, IEquatable<AuthenticationStartedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AuthenticationStartedEvent](Void.Proxy.Api.Events.Authentication.AuthenticationStartedEvent.md)

#### Implements

[IScopedEventWithResult<AuthenticationResult\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<AuthenticationResult\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<AuthenticationStartedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartedEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Events_Authentication_AuthenticationSide_"></a> AuthenticationStartedEvent\(ILink, IPlayer, AuthenticationSide\)

```csharp
public AuthenticationStartedEvent(ILink Link, IPlayer Player, AuthenticationSide Side)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Side` [AuthenticationSide](Void.Proxy.Api.Events.Authentication.AuthenticationSide.md)

## Properties

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartedEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartedEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public AuthenticationResult? Result { get; set; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)?

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationStartedEvent_Side"></a> Side

```csharp
public AuthenticationSide Side { get; init; }
```

#### Property Value

 [AuthenticationSide](Void.Proxy.Api.Events.Authentication.AuthenticationSide.md)

