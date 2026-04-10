# <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent"></a> Class MessageReceivedEvent

Namespace: [Void.Proxy.Api.Events.Network](Void.Proxy.Api.Events.Network.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record MessageReceivedEvent : IScopedEventWithResult<bool>, IScopedEvent, IEventWithResult<bool>, IEvent, IEquatable<MessageReceivedEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MessageReceivedEvent](Void.Proxy.Api.Events.Network.MessageReceivedEvent.md)

#### Implements

[IScopedEventWithResult<bool\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<bool\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<MessageReceivedEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent__ctor_Void_Proxy_Api_Network_Side_Void_Proxy_Api_Network_Side_Void_Proxy_Api_Network_Side_Void_Proxy_Api_Network_Direction_Void_Proxy_Api_Network_Messages_INetworkMessage_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_"></a> MessageReceivedEvent\(Side, Side, Side, Direction, INetworkMessage, ILink, IPlayer\)

```csharp
public MessageReceivedEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link, IPlayer Player)
```

#### Parameters

`Origin` [Side](Void.Proxy.Api.Network.Side.md)

`From` [Side](Void.Proxy.Api.Network.Side.md)

`To` [Side](Void.Proxy.Api.Network.Side.md)

`Direction` [Direction](Void.Proxy.Api.Network.Direction.md)

`Message` [INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

## Properties

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Direction"></a> Direction

```csharp
public Direction Direction { get; init; }
```

#### Property Value

 [Direction](Void.Proxy.Api.Network.Direction.md)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_From"></a> From

```csharp
public Side From { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Message"></a> Message

```csharp
public INetworkMessage Message { get; init; }
```

#### Property Value

 [INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Origin"></a> Origin

```csharp
public Side Origin { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Result"></a> Result

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if packet should not be sent

```csharp
public bool Result { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_To"></a> To

```csharp
public Side To { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

## Methods

### <a id="Void_Proxy_Api_Events_Network_MessageReceivedEvent_Cancel"></a> Cancel\(\)

Cancels an operation if it hasn't been canceled already. It sets the Result to true upon cancellation.

```csharp
public bool Cancel()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Returns true if the operation was already canceled; otherwise, returns false.

