# <a id="Void_Minecraft_Events_PhaseChangedEvent"></a> Class PhaseChangedEvent

Namespace: [Void.Minecraft.Events](Void.Minecraft.Events.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record PhaseChangedEvent : IScopedEvent, IEvent, IEquatable<PhaseChangedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PhaseChangedEvent](Void.Minecraft.Events.PhaseChangedEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PhaseChangedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Events_PhaseChangedEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Network_Side_Void_Proxy_Api_Network_Channels_INetworkChannel_Void_Minecraft_Network_Phase_"></a> PhaseChangedEvent\(ILink?, IPlayer, Side, INetworkChannel, Phase\)

```csharp
public PhaseChangedEvent(ILink? Link, IPlayer Player, Side Side, INetworkChannel Channel, Phase Phase)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)?

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Side` [Side](Void.Proxy.Api.Network.Side.md)

`Channel` [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

`Phase` [Phase](Void.Minecraft.Network.Phase.md)

## Properties

### <a id="Void_Minecraft_Events_PhaseChangedEvent_Channel"></a> Channel

```csharp
public INetworkChannel Channel { get; init; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

### <a id="Void_Minecraft_Events_PhaseChangedEvent_Link"></a> Link

```csharp
public ILink? Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)?

### <a id="Void_Minecraft_Events_PhaseChangedEvent_Phase"></a> Phase

```csharp
public Phase Phase { get; init; }
```

#### Property Value

 [Phase](Void.Minecraft.Network.Phase.md)

### <a id="Void_Minecraft_Events_PhaseChangedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Minecraft_Events_PhaseChangedEvent_Side"></a> Side

```csharp
public Side Side { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

