# <a id="Void_Proxy_Api_Events_Channels_SearchChannelBuilderEvent"></a> Class SearchChannelBuilderEvent

Namespace: [Void.Proxy.Api.Events.Channels](Void.Proxy.Api.Events.Channels.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record SearchChannelBuilderEvent : IScopedEventWithResult<ChannelBuilder>, IScopedEvent, IEventWithResult<ChannelBuilder>, IEvent, IEquatable<SearchChannelBuilderEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SearchChannelBuilderEvent](Void.Proxy.Api.Events.Channels.SearchChannelBuilderEvent.md)

#### Implements

[IScopedEventWithResult<ChannelBuilder\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<ChannelBuilder\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<SearchChannelBuilderEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Channels_SearchChannelBuilderEvent__ctor_Void_Proxy_Api_Players_IPlayer_System_Memory_System_Byte__"></a> SearchChannelBuilderEvent\(IPlayer, Memory<byte\>\)

```csharp
public SearchChannelBuilderEvent(IPlayer Player, Memory<byte> Buffer)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Buffer` [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Proxy_Api_Events_Channels_SearchChannelBuilderEvent_Buffer"></a> Buffer

```csharp
public Memory<byte> Buffer { get; init; }
```

#### Property Value

 [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Proxy_Api_Events_Channels_SearchChannelBuilderEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Channels_SearchChannelBuilderEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public ChannelBuilder? Result { get; set; }
```

#### Property Value

 [ChannelBuilder](Void.Proxy.Api.Network.Channels.ChannelBuilder.md)?

