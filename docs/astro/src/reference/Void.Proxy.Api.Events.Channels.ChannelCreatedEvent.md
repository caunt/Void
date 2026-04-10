# <a id="Void_Proxy_Api_Events_Channels_ChannelCreatedEvent"></a> Class ChannelCreatedEvent

Namespace: [Void.Proxy.Api.Events.Channels](Void.Proxy.Api.Events.Channels.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record ChannelCreatedEvent : IScopedEvent, IEvent, IEquatable<ChannelCreatedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ChannelCreatedEvent](Void.Proxy.Api.Events.Channels.ChannelCreatedEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<ChannelCreatedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Channels_ChannelCreatedEvent__ctor_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Network_Side_Void_Proxy_Api_Network_Channels_INetworkChannel_"></a> ChannelCreatedEvent\(IPlayer, Side, INetworkChannel\)

```csharp
public ChannelCreatedEvent(IPlayer Player, Side Side, INetworkChannel Channel)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Side` [Side](Void.Proxy.Api.Network.Side.md)

`Channel` [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

## Properties

### <a id="Void_Proxy_Api_Events_Channels_ChannelCreatedEvent_Channel"></a> Channel

```csharp
public INetworkChannel Channel { get; init; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

### <a id="Void_Proxy_Api_Events_Channels_ChannelCreatedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Channels_ChannelCreatedEvent_Side"></a> Side

```csharp
public Side Side { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

