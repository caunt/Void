# <a id="Void_Proxy_Api_Events_Authentication_AuthenticationFinishedEvent"></a> Class AuthenticationFinishedEvent

Namespace: [Void.Proxy.Api.Events.Authentication](Void.Proxy.Api.Events.Authentication.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record AuthenticationFinishedEvent : IScopedEvent, IEvent, IEquatable<AuthenticationFinishedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AuthenticationFinishedEvent](Void.Proxy.Api.Events.Authentication.AuthenticationFinishedEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<AuthenticationFinishedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationFinishedEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Events_Authentication_AuthenticationSide_Void_Proxy_Api_Events_Authentication_AuthenticationResult_"></a> AuthenticationFinishedEvent\(ILink, IPlayer, AuthenticationSide, AuthenticationResult\)

```csharp
public AuthenticationFinishedEvent(ILink Link, IPlayer Player, AuthenticationSide Side, AuthenticationResult Result)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Side` [AuthenticationSide](Void.Proxy.Api.Events.Authentication.AuthenticationSide.md)

`Result` [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

## Properties

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationFinishedEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationFinishedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationFinishedEvent_Result"></a> Result

```csharp
public AuthenticationResult Result { get; init; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationFinishedEvent_Side"></a> Side

```csharp
public AuthenticationSide Side { get; init; }
```

#### Property Value

 [AuthenticationSide](Void.Proxy.Api.Events.Authentication.AuthenticationSide.md)

