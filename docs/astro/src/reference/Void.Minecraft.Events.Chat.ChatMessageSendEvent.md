# <a id="Void_Minecraft_Events_Chat_ChatMessageSendEvent"></a> Class ChatMessageSendEvent

Namespace: [Void.Minecraft.Events.Chat](Void.Minecraft.Events.Chat.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ChatMessageSendEvent : IScopedEventWithResult<ChatSendResult>, IScopedEvent, IEventWithResult<ChatSendResult>, IEvent, IEquatable<ChatMessageSendEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ChatMessageSendEvent](Void.Minecraft.Events.Chat.ChatMessageSendEvent.md)

#### Implements

[IScopedEventWithResult<ChatSendResult\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<ChatSendResult\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<ChatMessageSendEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Events_Chat_ChatMessageSendEvent__ctor_Void_Proxy_Api_Players_IPlayer_Void_Minecraft_Components_Text_Component_Void_Proxy_Api_Network_Side_"></a> ChatMessageSendEvent\(IPlayer, Component, Side\)

```csharp
public ChatMessageSendEvent(IPlayer Player, Component Text, Side Origin)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Text` [Component](Void.Minecraft.Components.Text.Component.md)

`Origin` [Side](Void.Proxy.Api.Network.Side.md)

## Properties

### <a id="Void_Minecraft_Events_Chat_ChatMessageSendEvent_Origin"></a> Origin

```csharp
public Side Origin { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

### <a id="Void_Minecraft_Events_Chat_ChatMessageSendEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Minecraft_Events_Chat_ChatMessageSendEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public ChatSendResult Result { get; set; }
```

#### Property Value

 [ChatSendResult](Void.Minecraft.Events.Chat.ChatSendResult.md)

### <a id="Void_Minecraft_Events_Chat_ChatMessageSendEvent_Text"></a> Text

```csharp
public Component Text { get; init; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)

