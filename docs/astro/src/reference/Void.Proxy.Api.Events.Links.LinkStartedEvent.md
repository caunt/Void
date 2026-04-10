# <a id="Void_Proxy_Api_Events_Links_LinkStartedEvent"></a> Class LinkStartedEvent

Namespace: [Void.Proxy.Api.Events.Links](Void.Proxy.Api.Events.Links.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record LinkStartedEvent : IScopedEvent, IEvent, IEquatable<LinkStartedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[LinkStartedEvent](Void.Proxy.Api.Events.Links.LinkStartedEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<LinkStartedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Links_LinkStartedEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_System_Boolean_"></a> LinkStartedEvent\(ILink, IPlayer, bool\)

```csharp
public LinkStartedEvent(ILink Link, IPlayer Player, bool IsFirstLink)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`IsFirstLink` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Void_Proxy_Api_Events_Links_LinkStartedEvent_IsFirstLink"></a> IsFirstLink

```csharp
public bool IsFirstLink { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Links_LinkStartedEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Links_LinkStartedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

