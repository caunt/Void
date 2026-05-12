# <a id="Void_Minecraft_Events_Chat_ChatCommandSendEvent"></a> Class ChatCommandSendEvent

Namespace: [Void.Minecraft.Events.Chat](Void.Minecraft.Events.Chat.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ChatCommandSendEvent : IScopedEventWithResult<ChatSendResult>, IScopedEvent, IEventWithResult<ChatSendResult>, IEvent, IEquatable<ChatCommandSendEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ChatCommandSendEvent](Void.Minecraft.Events.Chat.ChatCommandSendEvent.md)

#### Implements

[IScopedEventWithResult<ChatSendResult\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<ChatSendResult\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<ChatCommandSendEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Events_Chat_ChatCommandSendEvent__ctor_Void_Proxy_Api_Players_IPlayer_System_String_Void_Proxy_Api_Network_Side_"></a> ChatCommandSendEvent\(IPlayer, string, Side\)

```csharp
public ChatCommandSendEvent(IPlayer Player, string Command, Side Origin)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Command` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Origin` [Side](Void.Proxy.Api.Network.Side.md)

## Properties

### <a id="Void_Minecraft_Events_Chat_ChatCommandSendEvent_Command"></a> Command

```csharp
public string Command { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Events_Chat_ChatCommandSendEvent_Origin"></a> Origin

```csharp
public Side Origin { get; init; }
```

#### Property Value

 [Side](Void.Proxy.Api.Network.Side.md)

### <a id="Void_Minecraft_Events_Chat_ChatCommandSendEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Minecraft_Events_Chat_ChatCommandSendEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public ChatSendResult Result { get; set; }
```

#### Property Value

 [ChatSendResult](Void.Minecraft.Events.Chat.ChatSendResult.md)

