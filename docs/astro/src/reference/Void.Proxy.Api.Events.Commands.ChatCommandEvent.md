# <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent"></a> Class ChatCommandEvent

Namespace: [Void.Proxy.Api.Events.Commands](Void.Proxy.Api.Events.Commands.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record ChatCommandEvent : IScopedEventWithResult<bool>, IScopedEvent, IEventWithResult<bool>, IEvent, IEquatable<ChatCommandEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ChatCommandEvent](Void.Proxy.Api.Events.Commands.ChatCommandEvent.md)

#### Implements

[IScopedEventWithResult<bool\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<bool\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<ChatCommandEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_System_String_System_Boolean_"></a> ChatCommandEvent\(ILink, IPlayer, string, bool\)

```csharp
public ChatCommandEvent(ILink Link, IPlayer Player, string Command, bool IsSigned)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Command` [string](https://learn.microsoft.com/dotnet/api/system.string)

`IsSigned` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent_Command"></a> Command

```csharp
public string Command { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent_IsSigned"></a> IsSigned

```csharp
public bool IsSigned { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Commands_ChatCommandEvent_Result"></a> Result

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if command should not be sent to Server

```csharp
public bool Result { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

