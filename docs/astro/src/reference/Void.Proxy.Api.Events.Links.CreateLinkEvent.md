# <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent"></a> Class CreateLinkEvent

Namespace: [Void.Proxy.Api.Events.Links](Void.Proxy.Api.Events.Links.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record CreateLinkEvent : IScopedEventWithResult<ILink>, IScopedEvent, IEventWithResult<ILink>, IEvent, IEquatable<CreateLinkEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CreateLinkEvent](Void.Proxy.Api.Events.Links.CreateLinkEvent.md)

#### Implements

[IScopedEventWithResult<ILink\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<ILink\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<CreateLinkEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent__ctor_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Servers_IServer_Void_Proxy_Api_Network_Channels_INetworkChannel_Void_Proxy_Api_Network_Channels_INetworkChannel_"></a> CreateLinkEvent\(IPlayer, IServer, INetworkChannel, INetworkChannel\)

```csharp
public CreateLinkEvent(IPlayer Player, IServer Server, INetworkChannel PlayerChannel, INetworkChannel ServerChannel)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Server` [IServer](Void.Proxy.Api.Servers.IServer.md)

`PlayerChannel` [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

`ServerChannel` [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

## Properties

### <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent_PlayerChannel"></a> PlayerChannel

```csharp
public INetworkChannel PlayerChannel { get; init; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

### <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public ILink? Result { get; set; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)?

### <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent_Server"></a> Server

```csharp
public IServer Server { get; init; }
```

#### Property Value

 [IServer](Void.Proxy.Api.Servers.IServer.md)

### <a id="Void_Proxy_Api_Events_Links_CreateLinkEvent_ServerChannel"></a> ServerChannel

```csharp
public INetworkChannel ServerChannel { get; init; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

