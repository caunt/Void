# <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent"></a> Class PlayerJoinedServerEvent

Namespace: [Void.Proxy.Api.Events.Player](Void.Proxy.Api.Events.Player.md)  
Assembly: Void.Proxy.Api.dll  

Event that occurs when a player joins a server and both sides are in Play phase.

```csharp
public record PlayerJoinedServerEvent : IScopedEventWithResult<bool>, IScopedEvent, IEventWithResult<bool>, IEvent, IEquatable<PlayerJoinedServerEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerJoinedServerEvent](Void.Proxy.Api.Events.Player.PlayerJoinedServerEvent.md)

#### Implements

[IScopedEventWithResult<bool\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<bool\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PlayerJoinedServerEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent__ctor_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Servers_IServer_Void_Proxy_Api_Links_ILink_System_Boolean_"></a> PlayerJoinedServerEvent\(IPlayer, IServer, ILink, bool\)

Event that occurs when a player joins a server and both sides are in Play phase.

```csharp
public PlayerJoinedServerEvent(IPlayer Player, IServer Server, ILink Link, bool IsRedirected)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player who has joined the server. Cannot be null.

`Server` [IServer](Void.Proxy.Api.Servers.IServer.md)

The server that the player has joined. Cannot be null.

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

The link associated with the player's connection to the server. Cannot be null.

`IsRedirected` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the player arrived by being redirected from another server; <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> if this is the player's initial server connection.

## Properties

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent_IsRedirected"></a> IsRedirected

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the player arrived by being redirected from another server; <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> if this is the player's initial server connection.

```csharp
public bool IsRedirected { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent_Link"></a> Link

The link associated with the player's connection to the server. Cannot be null.

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent_Player"></a> Player

The player who has joined the server. Cannot be null.

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent_Result"></a> Result

Gets or sets a value indicating whether the player will be redirected after this event executes.

```csharp
public bool Result { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent_Server"></a> Server

The server that the player has joined. Cannot be null.

```csharp
public IServer Server { get; init; }
```

#### Property Value

 [IServer](Void.Proxy.Api.Servers.IServer.md)

### <a id="Void_Proxy_Api_Events_Player_PlayerJoinedServerEvent_WillBeRedirected"></a> WillBeRedirected

Gets or sets a value indicating whether the player will be redirected after this event executes.

```csharp
public bool WillBeRedirected { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

