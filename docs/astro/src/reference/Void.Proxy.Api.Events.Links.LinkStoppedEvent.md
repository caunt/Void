# <a id="Void_Proxy_Api_Events_Links_LinkStoppedEvent"></a> Class LinkStoppedEvent

Namespace: [Void.Proxy.Api.Events.Links](Void.Proxy.Api.Events.Links.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record LinkStoppedEvent : IScopedEvent, IEvent, IEquatable<LinkStoppedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[LinkStoppedEvent](Void.Proxy.Api.Events.Links.LinkStoppedEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<LinkStoppedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Links_LinkStoppedEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Links_LinkStopReason_"></a> LinkStoppedEvent\(ILink, IPlayer, LinkStopReason\)

```csharp
public LinkStoppedEvent(ILink Link, IPlayer Player, LinkStopReason Reason)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Reason` [LinkStopReason](Void.Proxy.Api.Links.LinkStopReason.md)

## Properties

### <a id="Void_Proxy_Api_Events_Links_LinkStoppedEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Links_LinkStoppedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Links_LinkStoppedEvent_Reason"></a> Reason

```csharp
public LinkStopReason Reason { get; init; }
```

#### Property Value

 [LinkStopReason](Void.Proxy.Api.Links.LinkStopReason.md)

