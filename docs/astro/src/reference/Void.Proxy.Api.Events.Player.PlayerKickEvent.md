# <a id="Void_Proxy_Api_Events_Player_PlayerKickEvent"></a> Class PlayerKickEvent

Namespace: [Void.Proxy.Api.Events.Player](Void.Proxy.Api.Events.Player.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record PlayerKickEvent : IScopedEventWithResult<bool>, IScopedEvent, IEventWithResult<bool>, IEvent, IEquatable<PlayerKickEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerKickEvent](Void.Proxy.Api.Events.Player.PlayerKickEvent.md)

#### Derived

[MinecraftPlayerKickEvent](Void.Minecraft.Events.MinecraftPlayerKickEvent.md)

#### Implements

[IScopedEventWithResult<bool\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<bool\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PlayerKickEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Player_PlayerKickEvent__ctor_Void_Proxy_Api_Players_IPlayer_System_String_"></a> PlayerKickEvent\(IPlayer, string?\)

```csharp
public PlayerKickEvent(IPlayer Player, string? Text = null)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Text` [string](https://learn.microsoft.com/dotnet/api/system.string)?

## Properties

### <a id="Void_Proxy_Api_Events_Player_PlayerKickEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Events_Player_PlayerKickEvent_Result"></a> Result

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if kick was made; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

```csharp
public bool Result { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Player_PlayerKickEvent_Text"></a> Text

```csharp
public string? Text { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

